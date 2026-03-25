# Team Tasks — IaC & CI/CD Milestone

## What This Is

Team Tasks is an ASP.NET Core + React application that needs production-grade Infrastructure as Code and automated build/deployment pipelines. This milestone adds OpenTofu-managed Azure infrastructure (Dev + Production environments), GitHub Actions CI/CD with full quality and security gate automation, a rolling deployment strategy for production, and repeatable environment provisioning — enabling the team to ship reliably and safely from day one.

## Core Value

Every code change must be validated by automated quality and security gates before it can be deployed, and every deployment must be repeatable, rollback-ready, and consistent across environments.

## Requirements

### Validated

- ✓ ASP.NET Core Web API on .NET 10 — existing
- ✓ React + Vite + TypeScript frontend — existing
- ✓ SQLite persistence via EF Core — existing
- ✓ xUnit backend unit tests — existing
- ✓ Vitest + RTL frontend tests — existing

### Active

- [ ] IaC for Azure Dev environment (OpenTofu)
- [ ] IaC for Azure Production environment (OpenTofu)
- [ ] GitHub Actions CI pipeline with quality and security gates
- [ ] GitHub Actions CD pipeline with rolling deployment to Azure
- [ ] Container build and image registry workflow
- [ ] Secret and environment variable management via Azure Key Vault and GitHub secrets
- [ ] SAST and dependency vulnerability scanning in CI
- [ ] IaC policy/lint checks (tflint) in CI
- [ ] Manual approval gate for Production deployments
- [ ] Rollback capability for production deployments
- [ ] Environment parity between Dev and Production (same IaC modules, different vars)

### Out of Scope

- Multi-region or geo-distributed deployment — complexity not warranted for this milestone
- Kubernetes or AKS orchestration — App Service/Container Apps is sufficient for now
- Staging environment — Dev + Prod covers the stated goal; staging deferred to next milestone
- Blue/Green or Canary strategies — Rolling is the chosen strategy for this milestone
- Observability and alerting (App Insights, dashboards) — deferred to a dedicated observability milestone

## Context

The existing codebase is a clean monorepo:
- `server/TeamTasks.Api` — ASP.NET Core API, targets .NET 10
- `client` — React 19 + Vite 8 + TypeScript, pnpm workspaces
- `tests/TeamTasks.Api.Tests` — xUnit backend tests
- No CI/CD, no IaC, no containerization currently exists

**Decisions locked for this milestone:**
- Cloud provider: Azure
- IaC tool: OpenTofu (open-source Terraform fork)
- Environments: Dev (auto-deploy) + Production (manual approval gate)
- CI/CD host: GitHub Actions
- Release strategy: Rolling deployments
- Security gates: Unit tests, lint, SAST, dependency scanning, container image scanning, IaC lint, manual prod approval

## Constraints

- **IaC**: OpenTofu — not Terraform or Bicep; maintain provider parity with Azure RM
- **CI/CD**: GitHub Actions only — no Azure DevOps, CircleCI, or other platforms
- **Environments**: Dev and Production only for this milestone
- **Release strategy**: Rolling — no Blue/Green or Canary for v1
- **State backend**: Azure Storage Account for OpenTofu remote state

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| OpenTofu over Terraform | Open-source, BSL-free, full compatibility with AzureRM provider | — Pending |
| Azure App Service (or Container Apps) for hosting | Managed PaaS reduces operational overhead; no Kubernetes complexity | — Pending |
| GitHub Actions for CI/CD | Colocated with source code; native GitHub integration | — Pending |
| Rolling deployment strategy | Simpler than Blue/Green for this app's scale; near-zero downtime | — Pending |
| Manual approval gate for prod | Reduces production risk; aligns with stated goals | — Pending |
| Azure Storage as OpenTofu backend | Managed, durable, native Azure service | — Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-03-24 after initialization*
