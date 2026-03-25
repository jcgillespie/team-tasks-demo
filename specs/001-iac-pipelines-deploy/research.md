# Research: Deployment IaC and Pipelines

## Decision 1: Host the workload on AKS with separate frontend and API deployments

- Decision: Run the React frontend and ASP.NET Core API as separate containerized workloads on AKS, exposed through a single ingress entry point.
- Rationale: The requested target platform is AKS, and separate workloads preserve clear ownership boundaries, allow independent rollout behavior, and fit the existing split between client and server projects.
- Alternatives considered: Single combined container was rejected because it couples unrelated runtimes and release cadence. Azure App Service or Container Apps were rejected because the requested hosting target is AKS.

## Decision 2: Use Terraform for Azure infrastructure with environment composition

- Decision: Model Azure resources in Terraform using reusable modules plus per-environment composition for development, stage, and production.
- Rationale: This satisfies the explicit Terraform requirement while keeping shared resource definitions consistent and environment-specific configuration separate. It also supports safe re-runs and drift reduction.
- Alternatives considered: Bicep was rejected because the requested stack specifies Terraform. Hand-authored portal setup or shell scripts were rejected because they do not provide the required repeatability.

## Decision 3: Use GitHub Actions for build, validate, publish, and deploy promotion stages

- Decision: Implement GitHub Actions workflows that lint and test the app, build and publish container images to Azure Container Registry, run Terraform validation and apply steps, and promote releases across environments with environment protections.
- Rationale: GitHub Actions matches the repository host and the requested delivery platform. Native environment protection rules support approval checkpoints, and workflows can surface quality-gate failures clearly.
- Alternatives considered: Azure DevOps pipelines were rejected because the requested stack specifies GitHub Actions. Manual promotion steps were rejected because they do not satisfy repeatability or observability requirements.

## Decision 4: Use GitHub OIDC federation with Azure role assignments instead of long-lived secrets

- Decision: Authenticate GitHub Actions to Azure through OpenID Connect federation and least-privilege role assignments, with runtime application secrets sourced from Azure Key Vault or Kubernetes secrets populated from secure inputs.
- Rationale: This removes static cloud credentials from the repository and aligns with the requirement that sensitive values stay out of source control. It also reduces secret rotation burden.
- Alternatives considered: Service principal client secrets stored as GitHub secrets were rejected as a primary approach because they introduce secret lifecycle and leakage risk.

## Decision 5: Use ACR for image storage and AKS workload identity-friendly secret handling

- Decision: Store built images in Azure Container Registry and design Kubernetes deployment assets so application configuration can be supplied from environment-specific secrets and config maps, with a path to managed identity-backed secret access.
- Rationale: ACR integrates directly with AKS and GitHub Actions image publishing. Separating secret values from manifests keeps the deployment definition reusable across environments.
- Alternatives considered: Public registries were rejected because they weaken control over image access and do not match the Azure-centered deployment model.

## Decision 6: Treat promotion quality gates as externalized policy inputs

- Decision: The current feature will enforce promotion checkpoints that evaluate configurable quality-gate results, but the specific rule authoring model will remain external to this feature.
- Rationale: This respects the clarified scope boundary while still requiring the delivery design to support blocked promotions and clear operator feedback.
- Alternatives considered: Hard-coding gate criteria in workflows was rejected because the spec explicitly defers criteria definition. Ignoring gates until a later feature was rejected because promotion blocking is already a current requirement.

## Decision 7: Preserve the current application topology and avoid database redesign in this feature

- Decision: Plan around the existing API and frontend behavior without redesigning the application architecture, while documenting that the current SQLite persistence model requires explicit persistent-storage treatment in AKS.
- Rationale: The feature is about deployment enablement, not application redesign. Calling out the persistence implication prevents hidden operational drift.
- Alternatives considered: Migrating immediately to Azure SQL or PostgreSQL was rejected because it expands scope beyond deployment enablement. Running SQLite on ephemeral container storage was rejected because it risks data loss.

## Decision 8: Add deployment documentation and operator quickstart as first-class outputs

- Decision: Produce infrastructure, pipeline, and environment operation guidance as part of the feature artifacts and planned repo documentation updates.
- Rationale: The constitution requires documentation parity, and deployment work is unsafe without clear operator instructions.
- Alternatives considered: Relying on workflow YAML alone was rejected because it is insufficient for onboarding and incident handling.