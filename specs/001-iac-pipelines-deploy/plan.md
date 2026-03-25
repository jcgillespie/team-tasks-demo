# Implementation Plan: Deployment IaC and Pipelines

**Branch**: `001-iac-pipelines-deploy` | **Date**: 2026-03-25 | **Spec**: `/Users/joshgillespie/src/team-tasks-demo/speckit/specs/001-iac-pipelines-deploy/spec.md`
**Input**: Feature specification from `/specs/001-iac-pipelines-deploy/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add deployment infrastructure and delivery automation for the Team Tasks application by containerizing the React client and ASP.NET Core API, provisioning Azure infrastructure with Terraform, publishing images through GitHub Actions, and deploying to AKS across development, stage, and production with approval and configurable quality-gate enforcement points.

## Technical Context

**Language/Version**: C# .NET 10, TypeScript 6, React 19, HCL for Terraform, GitHub Actions YAML  
**Primary Dependencies**: ASP.NET Core Web API, EF Core with SQLite (current app state), React + Vite, Vitest, xUnit, Terraform AzureRM provider, AKS, ACR, Key Vault, GitHub Actions OIDC federation  
**Storage**: SQLite for current application state; Azure-managed persistent volume or equivalent Kubernetes-backed persistence configuration required for container hosting of the API data file in non-ephemeral environments  
**Testing**: `dotnet test TeamTasks.slnx`, `pnpm test`, client linting, Terraform validation, container image build verification, pipeline smoke deployment validation  
**Target Platform**: Linux containers on Azure Kubernetes Service, GitHub-hosted runners, Azure control plane  
**Project Type**: Web application with separate frontend and backend, plus infrastructure and CI/CD assets  
**Performance Goals**: Provision a new environment in under 30 minutes; keep deployment feedback within normal CI duration for a small demo app; keep promotion flow deterministic across 3 environments  
**Constraints**: Must use Terraform, GitHub Actions, Azure-hosted runtime, and AKS; must support development, stage, and production; production promotion requires approval; promotion gates must be configurable but criteria definition is out of scope; secrets must stay out of committed source  
**Scale/Scope**: Single repo, one API service, one frontend service, three AKS namespaces or logically isolated environment deployments, initial demo-scale traffic and small-team operations

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **I. Code Quality**: Planned changes separate infrastructure, pipeline, and application concerns into distinct assets; container and deployment configuration will avoid ad-hoc inline environment logic.
- [x] **II. TDD**: Tasks will require pipeline, container, and deployment validation tests before implementation completion; existing app tests remain part of release gates and coverage target remains enforceable for changed app code.
- [x] **III. Clean Documentation**: README, architecture notes, feature artifacts, and deployment operating docs are explicitly included in scope.
- [x] **IV. UX Consistency**: No user-facing interaction model changes are introduced beyond deployment behavior; API response and frontend state handling remain aligned with existing conventions.
- [x] **V. Simplicity**: The design adds only the minimum new infrastructure needed for AKS hosting, image distribution, secret handling, and multi-environment promotion.

## Project Structure

### Documentation (this feature)

```text
specs/001-iac-pipelines-deploy/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
client/
├── src/
├── package.json
└── vite.config.ts

server/
└── TeamTasks.Api/
    ├── Controllers/
    ├── Data/
    ├── Models/
    ├── Services/
    ├── Program.cs
    └── TeamTasks.Api.csproj

tests/
└── delivery/

docs/
└── architecture.md

infra/
├── terraform/
│   ├── modules/
│   └── environments/
└── kubernetes/

.github/
└── workflows/

scripts/
```

**Structure Decision**: Use the existing web-application split with new top-level `infra/` and `.github/workflows/` assets. This keeps application code isolated from infrastructure and delivery code while matching the repo’s current monorepo layout.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |
