# Roadmap: Team Tasks — IaC & CI/CD Milestone

**Milestone:** Infrastructure as Code & CI/CD Pipelines
**Goal:** Add Azure infrastructure automation, containerization, and GitHub Actions CI/CD pipelines so every code change is validated by automated gates and every deployment is repeatable, rollback-ready, and environment-consistent.
**Status:** In Progress
**Target:** Dev + Production environments, Azure, OpenTofu, GitHub Actions, Rolling deployments

---

## Phases

### Phase 1: Azure Infrastructure Foundation

**Goal:** Provision Dev and Production Azure environments with OpenTofu — resource groups, App Service (or Container Apps), Azure Container Registry, Key Vault, and remote state backend. Both environments use the same module structure with per-env variable files.

**Requirements:** INFRA-01, INFRA-02, INFRA-03, INFRA-04, INFRA-05, INFRA-06, INFRA-07, SEC-01, SEC-02, SEC-03, SEC-04

**Plans:** 4 plans
**Status:** complete

Plans:
- [x] 01-01-PLAN.md — Bootstrap script + OpenTofu module library (Wave 1)
- [x] 01-02-PLAN.md — Backend migration: SQLite → Azure SQL (Wave 1)
- [x] 01-03-PLAN.md — Env configs (dev+prod) + OIDC federation (Wave 2)
- [x] 01-04-PLAN.md — Verification checkpoint (Wave 3)

---

### Phase 2: Containerization

**Goal:** Containerize the backend API and frontend with production-ready multi-stage Dockerfiles, add a local Docker Compose stack for integration testing, and integrate image pushes to Azure Container Registry.

**Requirements:** CONTAINER-01, CONTAINER-02, CONTAINER-03, CONTAINER-04

**Plans:** TBD plans
**Status:** not started

Plans:
- [ ] TBD

---

### Phase 3: CI Pipeline

**Goal:** Build a GitHub Actions CI pipeline that runs on every pull request and push to main — backend tests (xUnit), frontend tests (Vitest), ESLint, SAST (CodeQL), dependency vulnerability scan, container image scan (Trivy), and OpenTofu lint (tflint + tofu validate). All checks surface as GitHub PR status checks.

**Requirements:** CI-01, CI-02, CI-03, CI-04, CI-05, CI-06, CI-07, CI-08, CI-09

**Plans:** TBD plans
**Status:** not started

Plans:
- [ ] TBD

---

### Phase 4: CD Pipeline — Dev

**Goal:** Automate rolling deployments to the Dev Azure environment on merge to main. Post-deploy smoke test confirms the deployed API is responding. Deployment failures notify via GitHub Actions.

**Requirements:** CD-DEV-01, CD-DEV-02, CD-DEV-03, CD-DEV-04

**Plans:** TBD plans
**Status:** not started

Plans:
- [ ] TBD

---

### Phase 5: CD Pipeline — Production

**Goal:** Implement manually-triggered production deployments with a required approval gate, rolling strategy with health check validation, post-deploy smoke tests, and a one-click rollback workflow.

**Requirements:** CD-PROD-01, CD-PROD-02, CD-PROD-03, CD-PROD-04, CD-PROD-05

**Plans:** TBD plans
**Status:** not started

Plans:
- [ ] TBD

---

## Requirements Coverage

| Phase | Requirements | Count |
|-------|-------------|-------|
| Phase 1 | INFRA-01..07, SEC-01..04 | 11 |
| Phase 2 | CONTAINER-01..04 | 4 |
| Phase 3 | CI-01..09 | 9 |
| Phase 4 | CD-DEV-01..04 | 4 |
| Phase 5 | CD-PROD-01..05 | 5 |
| **Total** | | **33** |

> Note: 31 v1 requirements from REQUIREMENTS.md; SEC-01/02/03/04 assigned to Phase 1 for co-location with IaC.

---

## Dependencies

```
Phase 1 (IaC Foundation)
  └──► Phase 2 (Containerization)
         └──► Phase 3 (CI Pipeline)
                ├──► Phase 4 (CD Dev)
                └──► Phase 5 (CD Prod)
```

- Phase 2 requires Phase 1: Dockerfiles push to ACR provisioned in Phase 1
- Phase 3 requires Phase 2: CI builds and scans containers from Phase 2
- Phase 4 requires Phase 3: CD triggers only after CI passes
- Phase 5 requires Phase 4: Prod pipeline based on same pattern as Dev pipeline

---

## Milestone Acceptance

The milestone is complete when:
1. `tofu apply` in `infra/envs/dev` or `infra/envs/prod` produces a fully working Azure environment from scratch
2. A pull request to main shows green status checks for: build, tests, lint, SAST, dep vuln scan, container scan, IaC lint
3. Merging to main automatically deploys to Dev with rolling strategy
4. A production deployment requires a manual approval before proceeding
5. Production rollback can be triggered manually with a single workflow dispatch
6. No secrets exist in source code or GitHub Actions secrets beyond OIDC federation identifiers
