# Team Tasks Architecture

## Overview

This starter app is a monorepo with a small ASP.NET Core API and a React frontend.
It is intentionally simple so later worktree-based agent tasks can extend it safely.

## Structure

- `server/TeamTasks.Api`: ASP.NET Core Web API on .NET 10 with EF Core + SQLite
- `client`: React + Vite + TypeScript frontend
- `tests/TeamTasks.Api.Tests`: xUnit tests for backend validation and task logic
- `docs`: short design and architecture notes
- `scripts`: local helper scripts for common workflows

## Backend

- Entity model: `TaskItem`
- Persistence: `AppDbContext` using SQLite (`teamtasks.db`)
- API routes:
  - `GET /api/tasks`
  - `POST /api/tasks`
  - `PATCH /api/tasks/{id}/toggle`
- Validation: request data annotations through `[ApiController]`
- Seed: `DbInitializer` inserts sample tasks on first run
- CORS: configured from `appsettings.json` for Vite dev server

## Frontend

- Feature-oriented structure:
  - `src/api/tasksApi.ts`
  - `src/types/task.ts`
  - `src/components/TaskForm.tsx`
  - `src/components/TaskList.tsx`
  - `src/pages/TasksPage.tsx`
- State and side effects are local to `TasksPage` (`useState` + `useEffect`)
- Includes loading and error handling for fetch and update flows

## Testing

- Backend: xUnit + EF Core InMemory provider for service logic tests
- Frontend: Vitest + React Testing Library for baseline UI behavior

---

## Deployment Architecture

### Overview

The Team Tasks application is containerized and deployed to Azure Kubernetes Service (AKS) across three environments: **development**, **stage**, and **production**. Infrastructure is provisioned with Terraform, images are published through GitHub Actions, and environment promotion uses quality gates with manual approval for production.

### Infrastructure Topology

```text
GitHub Actions
  ├── validate.yml          ← PR validation (lint + test)
  ├── build.yml             ← Build + publish images to ACR
  ├── deploy-dev.yml        ← Auto-deploy to development
  ├── promote-to-stage.yml  ← Quality-gated promotion to stage
  └── promote-to-production.yml ← Approval-gated promotion to production

Azure Subscription
  ├── Resource Group: teamtasks-dev
  │   ├── AKS Cluster
  │   │   ├── Namespace: development
  │   │   │   ├── Deployment: teamtasks-api
  │   │   │   ├── Deployment: teamtasks-client
  │   │   │   └── Ingress: teamtasks-ingress
  │   ├── Azure Container Registry (ACR)
  │   ├── Virtual Network
  │   ├── Managed Identity
  │   └── Key Vault
  ├── Resource Group: teamtasks-stage (same structure)
  └── Resource Group: teamtasks-prod  (same structure + approval gate)
```

### Terraform Module Structure

```text
infra/terraform/
├── backend.tf                        ← Remote state (Azure Storage)
├── providers.tf                      ← AzureRM provider config
├── variables.tf                      ← Shared variable schema
├── outputs.tf                        ← Infrastructure output definitions
├── modules/
│   ├── aks/                          ← AKS cluster module
│   ├── container-registry/           ← ACR module
│   ├── networking/                   ← VNet + subnets module
│   ├── identity/                     ← Managed identity + RBAC module
│   └── secrets/                      ← Key Vault module
└── environments/
    ├── development/                  ← Dev environment root module
    ├── stage/                        ← Stage environment root module
    └── production/                   ← Production environment root module
```

### Container Images

| Service | Base Image | Registry Path |
|---------|-----------|--------------|
| API | `mcr.microsoft.com/dotnet/aspnet:10.0` | `<acr>.azurecr.io/teamtasks/api:<tag>` |
| Client | `nginx:1.27-alpine` | `<acr>.azurecr.io/teamtasks/client:<tag>` |

### Secrets and Identity

- GitHub Actions authenticates to Azure via **OIDC federation** (no long-lived credentials)
- Application secrets are stored in **Azure Key Vault** and never committed to source
- AKS workloads use **Managed Identity** for Key Vault access
- See [`infra/OIDC_SETUP.md`](../infra/OIDC_SETUP.md) for federation configuration

### Promotion Flow

```
Code commit
    │
    ▼
validate.yml (PR)      ← lint + test gate
    │
    ▼
build.yml (main)       ← immutable image published to ACR
    │
    ▼
deploy-dev.yml         ← auto-deploy to development namespace
    │
    ▼ quality gates pass
promote-to-stage.yml   ← same artifact promoted to stage namespace
    │
    ▼ quality gates pass + manual approval
promote-to-production.yml  ← same artifact promoted to production namespace
```

### Persistence Note

The current application uses **SQLite**. In AKS, the API pod requires a persistent volume to avoid data loss across restarts. The Terraform modules provision the necessary storage class; the Kubernetes API deployment manifest mounts a persistent volume claim. A future feature should migrate to Azure Database for PostgreSQL for production-grade persistence.
