# Implementation Plan: Infrastructure Delivery Automation

**Branch**: `001-iac-cicd-pipelines` | **Date**: 2026-03-23 | **Spec**: `/Users/joshgillespie/src/team-tasks-demo/speckit/specs/001-iac-cicd-pipelines/spec.md`
**Input**: Feature specification from `/Users/joshgillespie/src/team-tasks-demo/speckit/specs/001-iac-cicd-pipelines/spec.md`

## Summary

Add an Azure delivery foundation for the existing React + ASP.NET Core application using OpenTofu-managed infrastructure and GitHub Actions automation. The plan standardizes dev, staging, and prod on Azure App Service-hosted frontend and API workloads, Azure SQL for hosted persistence, Key Vault-managed secrets, and Application Insights-backed observability. Delivery is split into coordinated infrastructure and application workflows with OIDC authentication, saved plan/apply boundaries, staged promotion, drift detection, and rollback-ready release artifacts.

## Technical Context

**Language/Version**: .NET 10, TypeScript 5.9, Node.js 22+, OpenTofu 1.8+, GitHub Actions workflow YAML  
**Primary Dependencies**: ASP.NET Core Web API, EF Core 10, React 19, Vite 8, Vitest, xUnit, AzureRM/OpenTofu providers, `azure/login`, `azure/webapps-deploy`  
**Storage**: Local SQLite for developer runs; Azure SQL Database for hosted environments; Azure Storage containers for OpenTofu remote state and retained deployment manifests  
**Testing**: `pnpm lint`, `pnpm build`, `pnpm test`, `dotnet build TeamTasks.slnx`, `dotnet test TeamTasks.slnx`, `tofu fmt -check`, `tofu validate`, saved-plan verification, deployment smoke checks  
**Target Platform**: GitHub-hosted runners deploying to Azure Linux App Service, Azure SQL Database, Azure Key Vault, Azure Application Insights, Azure Log Analytics, and Azure Storage across dev/staging/prod  
**Project Type**: Full-stack monorepo web application with infrastructure and delivery automation assets  
**Performance Goals**: 95% of pull requests complete full CI in 15 minutes or less; new environment bootstrap in under 60 minutes; rollback in 15 minutes or less; no rebuild between stage promotions  
**Constraints**: OIDC and managed identity only for cloud auth, no preview environments, isolated per-environment state, separate but coordinated infra and app workflows, low-blast-radius rollout, documentation shipped with the same feature set  
**Scale/Scope**: One React frontend and one ASP.NET Core API promoted across three lifecycle environments with shared repo ownership and approval-based production governance

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Code Quality Gate**: Pass. PR validation will run client lint/build/test, solution build/test, OpenTofu formatting and validation for infra changes, and workflow-level artifact/plan checks before merge.
- **TDD Gate**: Pass. Implementation tasks must begin with failing-first coverage for workflow guardrails, infra validation paths, and deployment rollback behavior before enabling apply or promotion steps.
- **Documentation Gate**: Pass. Delivery will update `/README.md`, `/docs/architecture.md`, feature artifacts in `/specs/001-iac-cicd-pipelines/`, and new operational runbooks for deployment, rollback, drift response, and secret rotation.
- **UX Consistency Gate**: Pass. No end-user UI redesign is planned; contributor-facing workflow summaries must present clear success, failure, and next-action states in a consistent GitHub Actions summary format.
- **Simplicity Gate**: Pass. Azure App Service is preferred over Container Apps, AKS, or a split Static Web Apps topology to avoid premature containerization, extra deployment tokens, and unnecessary operational branching.

## Project Structure

### Documentation (this feature)

```text
specs/001-iac-cicd-pipelines/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── app-release-contract.md
│   └── infra-workflow-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
.github/
└── workflows/
    ├── ci.yml
    ├── infra-plan.yml
    ├── infra-apply.yml
    ├── release.yml
    └── drift-detection.yml

client/
├── src/
├── package.json
└── pnpm-lock.yaml

server/
└── TeamTasks.Api/

tests/
└── TeamTasks.Api.Tests/

infra/
└── opentofu/
    ├── modules/
    ├── environments/
    │   ├── dev/
    │   ├── staging/
    │   └── prod/
    └── backend/

docs/
├── architecture.md
└── operations/
    ├── deployment.md
    ├── rollback.md
    ├── drift-response.md
    └── secret-rotation.md

scripts/
└── dev.sh
```

**Structure Decision**: Keep the existing monorepo layout for application code and add a single `/infra/opentofu` tree plus GitHub workflow definitions under `/.github/workflows`. This preserves local developer ergonomics, keeps infrastructure isolated from runtime code, and maps cleanly to separate infra and application delivery pipelines.

## Complexity Tracking

No constitutional violations are currently justified. The plan intentionally avoids new runtime services such as containers, API gateways, or preview environments until the simpler App Service-based delivery path is in place.

## Post-Design Constitution Check

- **Code Quality Gate**: Still passes. Planned workflows cover existing repository commands and add OpenTofu validation without weakening current checks.
- **TDD Gate**: Still passes. Design artifacts explicitly require failing-first validation for pipeline guards, infra plan/apply safety, and rollback rehearsal.
- **Documentation Gate**: Still passes. Research, data model, contracts, and quickstart artifacts are generated in this phase, with implementation docs called out for follow-up.
- **UX Consistency Gate**: Still passes. Workflow summaries and runbooks are the only contributor-facing UX changes in scope.
- **Simplicity Gate**: Still passes. The selected architecture minimizes new abstractions while still meeting environment isolation, observability, and governance needs.
