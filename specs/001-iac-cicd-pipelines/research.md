# Phase 0 Research

## Decision 1: Use Azure App Service for both frontend and API hosting

- **Decision**: Provision one Linux App Service plan per environment and host two web apps in that plan: a frontend web app for the built React assets and an API web app for the ASP.NET Core service. Enable managed identity on both apps, and use a staging slot for the production API and frontend apps to support low-risk swaps.
- **Rationale**: This repository is already a non-containerized React + ASP.NET Core application. App Service aligns with the current build outputs, supports GitHub Actions OIDC natively, supports Key Vault references, and provides an easier first production target than introducing Docker, Container Apps revisions, or AKS.
- **Alternatives considered**:
  - Azure Container Apps: rejected for Phase 1 because it forces containerization, image governance, and more moving parts than the current app needs.
  - Azure Static Web Apps + App Service API: rejected because it introduces two deployment models and can require token-based deployment flows that conflict with the no-long-lived-credentials requirement.
  - AKS: rejected as operationally disproportionate for a small monorepo web application.

## Decision 2: Use Azure SQL Database for hosted persistence while keeping SQLite for local development

- **Decision**: Keep SQLite for local developer and test workflows, but model Azure SQL Database as the hosted database for dev, staging, and prod.
- **Rationale**: The local app currently relies on SQLite, which is fine for local runs but is not a safe production persistence choice on App Service. Azure SQL provides backups, operational durability, access control, and environment isolation without requiring a new data access pattern in local development.
- **Alternatives considered**:
  - Continue using SQLite in Azure: rejected because App Service file persistence and concurrent access characteristics are unsuitable for reliable hosted use.
  - Azure Cosmos DB: rejected because the current app does not need document-style modeling or the extra operational and cost footprint.

## Decision 3: Use Azure Key Vault, Application Insights, and Log Analytics as shared platform services per environment

- **Decision**: Provision one Key Vault, one Application Insights resource, and one Log Analytics workspace per environment. Frontend and API web apps use managed identity to resolve secret references and emit telemetry to the same environment-local monitoring stack.
- **Rationale**: This gives clean secret separation and environment-local observability while keeping the operational model straightforward. Official Microsoft guidance strongly supports managed identity and Key Vault references for App Service-hosted apps.
- **Alternatives considered**:
  - Store secrets directly in GitHub secrets: rejected because the feature explicitly requires platform secret storage and short-lived cloud auth.
  - Centralized shared vault across all environments: rejected because it weakens blast-radius isolation and makes approval boundaries less clear.

## Decision 4: Use OpenTofu modules plus per-environment roots with isolated remote state

- **Decision**: Create `/infra/opentofu/modules` for reusable Azure building blocks and `/infra/opentofu/environments/{dev,staging,prod}` for environment roots. Each environment uses its own backend config and state container in Azure Storage rather than CLI workspaces.
- **Rationale**: Separate environment roots make environment boundaries explicit in both local runs and GitHub Actions. A saved plan/apply workflow is easier to reason about when each environment has its own backend configuration, state key, and variables.
- **Alternatives considered**:
  - A single root with Terraform/OpenTofu workspaces: rejected because workspace switching is easy to misuse and hides environment context in CI.
  - Separate repos per environment: rejected because the current repo is small and shared modules already provide the necessary isolation.

## Decision 5: Split automation into PR CI, infrastructure plan/apply, application release, and drift detection workflows

- **Decision**: Implement four coordinated GitHub Actions workflows: `ci.yml`, `infra-plan.yml` plus `infra-apply.yml`, `release.yml`, and `drift-detection.yml`.
- **Rationale**: The specification requires separate but coordinated infrastructure and application delivery. Splitting workflows keeps feedback fast on PRs, allows saved plans and approvals for infra changes, and ensures application promotion uses the same immutable build output across environments.
- **Alternatives considered**:
  - Single end-to-end workflow: rejected because it increases blast radius, makes approvals harder to reason about, and slows feedback loops.
  - Manual infrastructure outside CI/CD: rejected because it undermines auditability, repeatability, and drift control.

## Decision 6: Authenticate GitHub Actions to Azure with GitHub Environments and OIDC federated credentials

- **Decision**: Configure one GitHub Environment per stage (`dev`, `staging`, `prod`) and bind each to a dedicated federated credential in Microsoft Entra ID. GitHub Actions uses `azure/login` with `id-token: write`; Azure RBAC is scoped per environment resource group.
- **Rationale**: This satisfies the short-lived-authentication requirement, keeps approval gates close to the environments they protect, and avoids publish profiles or service-principal secrets.
- **Alternatives considered**:
  - Publish profiles or long-lived client secrets: rejected because they violate the security requirement.
  - A single unscoped Azure identity for all environments: rejected because it broadens blast radius and weakens separation of duties.

## Decision 7: Promote immutable application packages, not rebuilt stage-specific artifacts

- **Decision**: Build the frontend bundle and API publish output once per release candidate, generate a deployment manifest containing version, commit SHA, checksums, and package locations, then promote that same artifact set through dev, staging, and prod.
- **Rationale**: The specification requires traceable releases and safer rollbacks. Rebuilding per environment would introduce avoidable drift between the artifact validated in lower environments and the artifact deployed to production.
- **Alternatives considered**:
  - Rebuild on each environment promotion: rejected because it weakens traceability and increases release variance.
  - Depend only on GitHub Actions artifact names: rejected because names alone are not sufficient rollback metadata.

## Decision 8: Implement scheduled drift detection with alerting and a documented remediation path

- **Decision**: Add a nightly `drift-detection.yml` workflow that runs `tofu init`, `tofu validate`, and `tofu plan` against each environment without applying changes. Any non-empty plan becomes an alert and an operational follow-up item.
- **Rationale**: Drift detection is an explicit functional requirement. A scheduled workflow gives fast visibility without introducing automatic corrective changes that could worsen an incident.
- **Alternatives considered**:
  - No scheduled drift process: rejected because drift is explicitly in scope.
  - Automatic drift remediation: rejected for the initial rollout because the blast radius is too high for a new delivery foundation.

## Confirmed Repository Commands

- **Frontend install**: `pnpm install`
- **Frontend lint**: `pnpm lint`
- **Frontend build**: `pnpm build`
- **Frontend tests**: `pnpm test`
- **Backend restore/build/test**: `dotnet restore TeamTasks.slnx`, `dotnet build TeamTasks.slnx`, `dotnet test TeamTasks.slnx`
- **Backend project target**: `server/TeamTasks.Api/TeamTasks.Api.csproj` on `.NET 10`

## Resolved Clarifications

- Cloud choice resolved to Azure.
- IaC tool choice resolved to OpenTofu with AzureRM-compatible module structure.
- Workflow split resolved to distinct CI, infrastructure, release, and drift-detection pipelines.
- Hosted persistence resolved to Azure SQL for cloud environments.

No material planning clarifications remain unresolved.