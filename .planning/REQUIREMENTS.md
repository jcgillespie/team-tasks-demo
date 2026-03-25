# Requirements: Team Tasks — IaC & CI/CD Milestone

**Defined:** 2026-03-24
**Core Value:** Every code change must be validated by automated quality and security gates before deployment, and every deployment must be repeatable, rollback-ready, and consistent across environments.

## v1 Requirements

### Infrastructure

- [ ] **INFRA-01**: Developer can provision a complete Dev environment on Azure by running a single OpenTofu command
- [ ] **INFRA-02**: Developer can provision a complete Production environment on Azure by running a single OpenTofu command
- [ ] **INFRA-03**: OpenTofu state is stored in an Azure Storage Account backend (remote state, locking enabled)
- [ ] **INFRA-04**: Dev and Production environments use the same OpenTofu module structure with per-environment variable files
- [ ] **INFRA-05**: All Azure resources are tagged with environment, project, and managed-by labels
- [ ] **INFRA-06**: Application secrets (DB connection strings, API keys) are stored in Azure Key Vault and referenced by the app at runtime
- [ ] **INFRA-07**: Infrastructure can be destroyed cleanly with a single OpenTofu destroy command without manual cleanup

### Containerization

- [ ] **CONTAINER-01**: Backend API has a production-ready Dockerfile with multi-stage build (build + runtime layers)
- [ ] **CONTAINER-02**: Frontend has a production-ready Dockerfile with multi-stage build (Node build + Nginx serve)
- [ ] **CONTAINER-03**: Docker Compose file allows running the full stack locally for integration testing
- [ ] **CONTAINER-04**: Built images are pushed to Azure Container Registry on successful CI runs

### CI Pipeline

- [ ] **CI-01**: Pull request to main triggers automated build verification for backend and frontend
- [ ] **CI-02**: CI runs all backend unit tests (xUnit) and fails the build if any test fails
- [ ] **CI-03**: CI runs all frontend tests (Vitest) and fails the build if any test fails
- [ ] **CI-04**: CI runs ESLint on the frontend codebase and fails on lint errors
- [ ] **CI-05**: CI runs a SAST scan (e.g., CodeQL or equivalent) on the codebase
- [ ] **CI-06**: CI runs a dependency vulnerability scan (e.g., dotnet audit + npm audit / OWASP Dependency Check)
- [ ] **CI-07**: CI runs container image vulnerability scanning (e.g., Trivy) after image build
- [ ] **CI-08**: CI runs OpenTofu lint/validate (tflint + tofu validate) on infrastructure code
- [ ] **CI-09**: CI run results (pass/fail, test counts) are visible in the GitHub pull request check UI

### CD Pipeline — Dev

- [ ] **CD-DEV-01**: Merge to main automatically triggers deployment to the Dev environment
- [ ] **CD-DEV-02**: Dev deployment uses rolling strategy (new instances started before old instances terminate)
- [ ] **CD-DEV-03**: Post-deploy smoke test confirms the deployed API is responding on the Dev environment
- [ ] **CD-DEV-04**: Dev deployment failure notifies the team (GitHub Actions failure + optional notification)

### CD Pipeline — Production

- [ ] **CD-PROD-01**: Production deployment is triggered manually (workflow_dispatch) or via a stable tag/release
- [ ] **CD-PROD-02**: Production deployment requires a manual approval gate before proceeding
- [ ] **CD-PROD-03**: Production deployment uses rolling strategy with health check validation at each step
- [ ] **CD-PROD-04**: Production deployment can be rolled back to the previous version with a single manual trigger
- [ ] **CD-PROD-05**: Post-deploy smoke test confirms the deployed API is responding on the Production environment

### Security & Secrets

- [ ] **SEC-01**: No secrets are stored in source code or committed to the repository
- [ ] **SEC-02**: GitHub Actions workflows use OIDC federation to authenticate to Azure (no long-lived service principal secrets)
- [ ] **SEC-03**: Azure Key Vault is used for all runtime secrets; applications reference vault URIs, not raw values
- [ ] **SEC-04**: Least-privilege RBAC assignments are applied to all managed identities and service accounts

## v2 Requirements

### Observability

- **OBS-01**: Application Insights is provisioned and configured for backend telemetry
- **OBS-02**: Deployment metrics (duration, success rate, rollback frequency) are tracked
- **OBS-03**: Alerting rules fire on deployment failure or post-deploy health check failure

### Environments

- **ENV-01**: Staging environment is provisioned as a pre-production validation environment
- **ENV-02**: Dev-to-Staging-to-Prod promotion flow is enforced by the pipeline

### Advanced Deployment

- **DEPLOY-01**: Blue/Green deployment option is available for zero-downtime production releases
- **DEPLOY-02**: Canary deployment option is available to reduce blast radius of production changes

## Out of Scope

| Feature | Reason |
|---------|--------|
| Kubernetes / AKS | PaaS hosting is sufficient for this app's scale; avoids Kubernetes complexity |
| Multi-region deployment | Not required for this milestone |
| Staging environment | Deferred to v2; Dev + Prod covers stated milestone goals |
| Blue/Green or Canary strategies | Rolling chosen for this milestone; Blue/Green and Canary deferred to v2 |
| Observability and dashboards | Deferred to a dedicated observability milestone |
| Azure DevOps pipelines | GitHub Actions is the locked choice for this milestone |
| Terraform (HashiCorp) | OpenTofu is the locked IaC tool; Terraform BSL-incompatible |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| INFRA-01 | Phase 1 | Pending |
| INFRA-02 | Phase 1 | Pending |
| INFRA-03 | Phase 1 | Pending |
| INFRA-04 | Phase 1 | Pending |
| INFRA-05 | Phase 1 | Pending |
| INFRA-06 | Phase 1 | Pending |
| INFRA-07 | Phase 1 | Pending |
| CONTAINER-01 | Phase 2 | Pending |
| CONTAINER-02 | Phase 2 | Pending |
| CONTAINER-03 | Phase 2 | Pending |
| CONTAINER-04 | Phase 2 | Pending |
| CI-01 | Phase 3 | Pending |
| CI-02 | Phase 3 | Pending |
| CI-03 | Phase 3 | Pending |
| CI-04 | Phase 3 | Pending |
| CI-05 | Phase 3 | Pending |
| CI-06 | Phase 3 | Pending |
| CI-07 | Phase 3 | Pending |
| CI-08 | Phase 3 | Pending |
| CI-09 | Phase 3 | Pending |
| CD-DEV-01 | Phase 4 | Pending |
| CD-DEV-02 | Phase 4 | Pending |
| CD-DEV-03 | Phase 4 | Pending |
| CD-DEV-04 | Phase 4 | Pending |
| CD-PROD-01 | Phase 5 | Pending |
| CD-PROD-02 | Phase 5 | Pending |
| CD-PROD-03 | Phase 5 | Pending |
| CD-PROD-04 | Phase 5 | Pending |
| CD-PROD-05 | Phase 5 | Pending |
| SEC-01 | Phase 1 | Pending |
| SEC-02 | Phase 1 | Pending |
| SEC-03 | Phase 1 | Pending |
| SEC-04 | Phase 1 | Pending |

**Coverage:**
- v1 requirements: 31 total
- Mapped to phases: 31
- Unmapped: 0 ✓

---
*Requirements defined: 2026-03-24*
*Last updated: 2026-03-24 after initial definition*
