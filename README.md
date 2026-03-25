# Team Tasks (Base App)

Clean starter app for future agentic workflow demos using Git worktrees.

## Folder Structure

```text
main/
	app/
	server/
		TeamTasks.Api/
	client/
	tests/
		TeamTasks.Api.Tests/
	docs/
	scripts/
	Prompts/
	TeamTasks.slnx
```

## Prerequisites

- .NET SDK 10
- Node.js 22+
- `pnpm` 10+

Install `pnpm` globally if needed:

```bash
corepack enable pnpm
```

## Local Setup

```bash
dotnet restore TeamTasks.slnx
cd client
pnpm install
```

## Run Backend

```bash
cd server/TeamTasks.Api
dotnet run
```

API URLs:

- `http://localhost:5276`
- OpenAPI JSON (dev): `http://localhost:5276/openapi/v1.json`
- Swagger UI (dev): `http://localhost:5276/swagger`

## Run Frontend

```bash
cd client
pnpm dev
```

---

## Deployment Prerequisites

> For full deployment setup, see [`infra/ENVIRONMENT_SETUP.md`](infra/ENVIRONMENT_SETUP.md).

### Required Tools

| Tool | Version | Purpose |
|------|---------|---------|
| Terraform | >= 1.6 | Azure infrastructure provisioning |
| Docker | >= 24 | Container image builds |
| Azure CLI (`az`) | >= 2.55 | Azure access and OIDC setup |
| `kubectl` | >= 1.28 | Kubernetes cluster interaction |
| `pnpm` | >= 10 | Frontend package management |
| .NET SDK | 10 | Backend build and test |

### Required Accounts / Access

- Azure subscription with permissions to create AKS, ACR, VNet, Key Vault, and identity resources
- GitHub repository admin access (for Actions environments, protection rules, and OIDC federation)

## Deployment Workflows Overview

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `validate.yml` | Pull request | Lint and test all code |
| `build.yml` | Push to `main` | Build and publish container images to ACR |
| `deploy-dev.yml` | After build | Auto-deploy latest artifact to development |
| `promote-to-stage.yml` | Manual / quality gate | Promote artifact from development → stage |
| `promote-to-production.yml` | Manual + approval | Promote artifact from stage → production |

## Deployment Quick Reference

```bash
# Provision a new environment (e.g., development)
cd infra/terraform/environments/development
terraform init -backend-config=../../backend.tf
terraform plan -out=tfplan
terraform apply tfplan

# Check environment status
bash scripts/deploy/environment-status.sh development

# Rollback to a previous artifact
bash scripts/deploy/rollback.sh development <artifact-id>
```

Full procedures: [`infra/ENVIRONMENT_SETUP.md`](infra/ENVIRONMENT_SETUP.md) | [`infra/TROUBLESHOOTING.md`](infra/TROUBLESHOOTING.md)

Frontend URL:

- `http://localhost:5173`

## Run Tests

Backend tests:

```bash
dotnet test TeamTasks.slnx
```

Frontend tests:

```bash
cd client
pnpm test
```

## Optional Helper Script

Start backend and frontend in separate Terminal windows:

```bash
./scripts/dev.sh
```

## Architecture Summary

- Backend: ASP.NET Core Web API (.NET 10), EF Core, SQLite, Swagger/OpenAPI.
- Frontend: React + Vite + TypeScript with a small feature-based structure.
- Data model: `Task` with `id`, `title`, `description`, `isCompleted`, `createdAt`.
- API endpoints:
	- `GET /api/tasks`
	- `POST /api/tasks`
	- `PATCH /api/tasks/{id}/toggle`
- Seed data: sample tasks are inserted on first run.
- Tests:
	- xUnit backend tests for validation, retrieval, and toggle behavior.
	- Vitest + RTL frontend tests for list rendering, creation, toggle, and loading/error states.

More detail: `docs/architecture.md`
