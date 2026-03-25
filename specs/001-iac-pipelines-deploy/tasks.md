# Implementation Tasks: Deployment IaC and Pipelines

**Feature**: `001-iac-pipelines-deploy`  
**Branch**: `001-iac-pipelines-deploy`  
**Status**: Ready for Implementation  
**Date Generated**: 2026-03-25

---

## Overview

This tasks document provides a dependency-ordered, independently testable task list for implementing deployment IaC and multi-environment promotion pipelines for the Team Tasks application on Azure AKS using Terraform and GitHub Actions.

### Task Execution Strategy

- **Phase 1 - Setup**: Establish directory structure and documentation foundation
- **Phase 2 - Foundational**: Build reusable infrastructure, pipeline, and container capabilities
- **Phase 3 - US1**: Implement environment provisioning with Terraform
- **Phase 4 - US2**: Implement automated build and deployment pipelines
- **Phase 5 - US3**: Implement safe promotion with quality gates and approvals
- **Phase 6 - Polish**: Complete documentation and operational guides

### Independence & Testing

Each phase is independently testable and can deliver value without later phases:
- **Phase 1+2** enable offline Terraform and Docker validation
- **Phase 1+2+3** enable environment provisioning testing
- **Phase 1+2+3+4** enable end-to-end build and deploy testing
- **Phase 1+2+3+4+5** enable full multi-environment promotion testing

---

## Phase 1: Setup

### 1.1 Create deployment directory structure

- [X] T001 Create infra/terraform/modules directory tree in repository root
- [X] T002 Create infra/terraform/environments directory tree for dev/stage/prod in repository root
- [X] T003 Create infra/kubernetes directory tree for manifests in repository root
- [X] T004 Create .github/workflows directory tree for pipeline automation in repository root
- [X] T005 Create scripts directory with deployment helper scripts in scripts/deploy
- [X] T006 Create tests/delivery directory for deployment and integration validation in tests/delivery

### 1.2 Initialize documentation foundation

- [X] T007 Create README section outlining deployment prerequisites and workflows in README.md
- [X] T008 Add deployment architecture section to docs/architecture.md
- [X] T009 Create environment bootstrap guide in infra/ENVIRONMENT_SETUP.md
- [X] T010 Create troubleshooting guide for deployment failures in infra/TROUBLESHOOTING.md

---

## Phase 2: Foundational

### 2.1 Terraform and infrastructure automation setup

- [X] T011 [P] Create Terraform backend configuration (state storage and locking) in infra/terraform/backend.tf
- [X] T012 [P] Create Terraform provider configuration for Azure in infra/terraform/providers.tf
- [X] T013 [P] Create Terraform variables schema file in infra/terraform/variables.tf
- [X] T014 [P] Create environment-specific terraform.tfvars template for development in infra/terraform/environments/development/terraform.tfvars.example
- [X] T015 [P] Create environment-specific terraform.tfvars template for stage in infra/terraform/environments/stage/terraform.tfvars.example
- [X] T016 [P] Create environment-specific terraform.tfvars template for production in infra/terraform/environments/production/terraform.tfvars.example
- [X] T017 Create Terraform output definitions capturing key infrastructure references in infra/terraform/outputs.tf
- [X] T018 Test: Terraform validate passes for all provider and variable configurations

### 2.2 Container image build and registry setup

- [X] T019 [P] Create Dockerfile for ASP.NET Core API service in server/Dockerfile
- [X] T020 [P] Create Dockerfile for React+Vite frontend service in client/Dockerfile
- [X] T021 [P] Add .dockerignore for API service in server/.dockerignore
- [X] T022 [P] Add .dockerignore for client service in client/.dockerignore
- [X] T023 Create Terraform module for Azure Container Registry (ACR) in infra/terraform/modules/container-registry/main.tf
- [X] T024 Test: Docker images build successfully for both API and client locally

### 2.3 GitHub Actions workflow foundation

- [X] T025 [P] Create GitHub Actions validation workflow (lint and test) in .github/workflows/validate.yml
- [X] T026 [P] Create GitHub Actions build workflow (container build and publish) in .github/workflows/build.yml
- [X] T027 Create GitHub Actions environment configuration for development in .github/workflows/environments/development.yml
- [X] T028 Create GitHub Actions environment configuration for stage in .github/workflows/environments/stage.yml
- [X] T029 Create GitHub Actions environment configuration for production in .github/workflows/environments/production.yml
- [X] T030 Create GitHub Actions promotion workflow template in .github/workflows/promote.yml
- [X] T031 Test: Validation workflow succeeds on clean code and main branch

---

## Phase 3: US1 - Provision a Repeatable Environment (P1)

### 3.1 Terraform modules for core infrastructure

- [X] T032 [P] [US1] Create Terraform module for AKS cluster provisioning in infra/terraform/modules/aks/main.tf
- [X] T033 [P] [US1] Create Terraform module variables for AKS configuration in infra/terraform/modules/aks/variables.tf
- [X] T034 [P] [US1] Create Terraform module outputs for AKS cluster details in infra/terraform/modules/aks/outputs.tf
- [X] T035 [P] [US1] Create Terraform module for virtual network and subnets in infra/terraform/modules/networking/main.tf
- [X] T036 [P] [US1] Create Terraform module for identity and role assignments in infra/terraform/modules/identity/main.tf
- [X] T037 [P] [US1] Create Terraform module for Key Vault and secret references in infra/terraform/modules/secrets/main.tf

### 3.2 Terraform environment composition

- [X] T038 [US1] Create development environment Terraform root module in infra/terraform/environments/development/main.tf
- [X] T039 [US1] Create stage environment Terraform root module in infra/terraform/environments/stage/main.tf
- [X] T040 [US1] Create production environment Terraform root module in infra/terraform/environments/production/main.tf
- [X] T041 [US1] Create environment-specific variable overrides for development in infra/terraform/environments/development/terraform.tfvars
- [X] T042 [US1] Create environment-specific variable overrides for stage in infra/terraform/environments/stage/terraform.tfvars
- [X] T043 [US1] Create environment-specific variable overrides for production in infra/terraform/environments/production/terraform.tfvars

### 3.3 Infrastructure provisioning validation and testing

- [X] T044 [US1] Test: Terraform plan succeeds for development environment without errors
- [X] T045 [US1] Test: Terraform plan succeeds for stage environment without errors
- [X] T046 [US1] Test: Terraform plan succeeds for production environment without errors
- [X] T047 [US1] Test: Infrastructure outputs (cluster name, registry URL, Key Vault reference) are available after provision
- [X] T048 [US1] Test: Re-running Terraform on existing environment produces no unintended duplicate resources
- [X] T049 [US1] Document: Deployment bootstrap procedure for all three environments in infra/ENVIRONMENT_SETUP.md

---

## Phase 4: US2 - Deploy from Source Through Automated Pipeline (P2)

### 4.1 GitHub Actions workflow implementation

- [X] T050 [P] [US2] Implement CI validation workflow to run linting and unit tests in .github/workflows/validate.yml
- [X] T051 [P] [US2] Implement container build workflow to build and publish images to ACR in .github/workflows/build.yml
- [X] T052 [US2] Implement deploy-to-development workflow triggered after successful build in .github/workflows/deploy-dev.yml
- [X] T053 [US2] Configure GitHub OIDC federation for Azure authentication in .github/workflows (GitHub Actions setup)
- [X] T054 [US2] Configure GitHub Actions secrets for Terraform backend and environment access in repository settings
- [X] T055 [US2] Create GitHub Actions reusable workflow for Terraform provisioning in .github/workflows/terraform.yml

### 4.2 Kubernetes deployment manifests

- [X] T056 [P] [US2] Create Kubernetes deployment manifest for API service in infra/kubernetes/api-deployment.yaml
- [X] T057 [P] [US2] Create Kubernetes deployment manifest for client service in infra/kubernetes/client-deployment.yaml
- [X] T058 [P] [US2] Create Kubernetes service configuration for API in infra/kubernetes/api-service.yaml
- [X] T059 [P] [US2] Create Kubernetes service configuration for client in infra/kubernetes/client-service.yaml
- [X] T060 [P] [US2] Create Kubernetes ingress configuration for external traffic routing in infra/kubernetes/ingress.yaml
- [X] T061 [P] [US2] Create Kubernetes namespace manifest for development environment in infra/kubernetes/namespaces/development.yaml
- [X] T062 [P] [US2] Create Kubernetes namespace manifest for stage environment in infra/kubernetes/namespaces/stage.yaml
- [X] T063 [P] [US2] Create Kubernetes namespace manifest for production environment in infra/kubernetes/namespaces/production.yaml

### 4.3 Pipeline and deployment validation

- [X] T064 [US2] Create script to validate pipeline output and capture deployment status in scripts/deploy/validate-deployment.sh
- [X] T065 [US2] Test: GitHub Actions validation workflow runs and passes on pull requests
- [X] T066 [US2] Test: GitHub Actions build workflow publishes images to ACR on main branch merge
- [X] T067 [US2] Test: Deployment to development environment succeeds after build completion
- [X] T068 [US2] Test: Deployed API service responds to health check requests in development
- [X] T069 [US2] Test: Deployed client service is accessible through ingress in development
- [X] T070 [US2] Test: Pipeline failure output clearly identifies the failed stage (validation, build, or deploy)

---

## Phase 5: US3 - Promote Changes Safely Across Environments (P3)

### 5.1 Multi-environment promotion workflow

- [X] T071 [US3] Implement promotion workflow to move release artifact from development to stage in .github/workflows/promote-to-stage.yml
- [X] T072 [US3] Implement promotion workflow to move release artifact from stage to production in .github/workflows/promote-to-production.yml
- [X] T073 [US3] Configure GitHub environment protection rules for production requiring manual approval in GitHub repository settings
- [X] T074 [US3] Create pull-request-based promotion request flow in .github/workflows/create-promotion-pr.yml
- [X] T075 [US3] Create quality-gate evaluation script in scripts/deploy/evaluate-quality-gates.sh

### 5.2 Quality gate configuration

- [X] T076 [US3] Create quality-gate configuration for development→stage transition in infra/quality-gates/dev-to-stage.yaml
- [X] T077 [US3] Create quality-gate configuration for stage→production transition in infra/quality-gates/stage-to-production.yaml
- [X] T078 [US3] Implement quality-gate evaluation in GitHub Actions promotion workflow
- [X] T079 [US3] Create clear failure message reporting when quality gates block promotion

### 5.3 Approval and promotion testing

- [X] T080 [US3] Test: Promotion from development to stage succeeds when quality gates pass
- [X] T081 [US3] Test: Promotion from stage to production is blocked without required approval
- [X] T082 [US3] Test: Promotion blocked by quality gate failure shows gate name and reason
- [X] T083 [US3] Test: Promotion records what was deployed and when in audit trail (workflow run history)
- [X] T084 [US3] Test: Same immutable artifact can be promoted without rebuilding
- [X] T085 [US3] Test: Environment-specific configuration is applied during promotion without changing artifact

---

## Phase 6: Polish & Cross-Cutting Concerns

### 6.1 Documentation completion

- [X] T086 Update repository README with deployment prerequisites section in README.md
- [X] T087 Update repository README with deployment procedures and commands in README.md
- [X] T088 Complete environment bootstrap guide with step-by-step instructions in infra/ENVIRONMENT_SETUP.md
- [X] T089 Complete troubleshooting guide with common deployment failure scenarios in infra/TROUBLESHOOTING.md
- [X] T090 Document GitHub Actions OIDC federation setup in infra/OIDC_SETUP.md
- [X] T091 Document quality-gate authoring guide for team maintenance in infra/QUALITY_GATES.md
- [X] T092 Update docs/architecture.md with complete AKS and deployment topology diagram in docs/architecture.md

### 6.2 Operational enablement

- [X] T093 Create Makefile or shell script wrapper for common provisioning commands in scripts/deploy/Makefile
- [X] T094 Create script to display environment status and resource health in scripts/deploy/environment-status.sh
- [X] T095 Create rollback procedure documentation and script in scripts/deploy/rollback.sh
- [X] T096 Create monitoring and alerting documentation for deployed applications in infra/MONITORING.md

### 6.3 Security and compliance hardening

- [X] T097 Verify no credentials or secrets appear in committed files (pre-commit hook or linting)
- [X] T098 Update .gitignore to exclude terraform state files and sensitive outputs in .gitignore
- [X] T099 Create RBAC documentation for least-privilege role assignments in infra/RBAC.md
- [X] T100 Document secret rotation and management procedures in infra/SECRET_MANAGEMENT.md

### 6.4 Final validation and closeout

- [X] T101 Test: Complete end-to-end deployment from code commit to production approval succeeds
- [X] T102 Test: All documentation is accurate and procedures are executable by a team member new to the repo
- [X] T103 Test: Deployment recovery from simulated failures (interrupted Terraform, failed pipeline stage, stuck promotion)
- [X] T104 Create deployment checklist for teams running the first provisioning and promotion in infra/DEPLOYMENT_CHECKLIST.md

---

## Task Dependency Graph

```
Phase 1 (Setup)
├─→ Phase 2 (Foundational)
    ├─→ Phase 3 (US1: Provisioning)
    │   └─→ Phase 4 (US2: Pipelines)
    │       └─→ Phase 5 (US3: Promotion)
    │           └─→ Phase 6 (Polish)
```

**No cross-phase dependencies**: Each phase can be completed independently, but executing them in order ensures infrastructure exists before pipeline deployment occurs.

---

## Parallel Execution Opportunities

### Within Phase 2:
- **T011-T017** (Terraform setup) can run independently
- **T019-T024** (Container images) can run independently  
- **T025-T031** (GitHub Actions templates) can run independently

### Within Phase 3:
- **T032-T037** (Infrastructure modules) can be developed in parallel
- **T038-T043** (Environment composition) can be tested in parallel after modules complete

### Within Phase 4:
- **T056-T063** (Kubernetes manifests) can be written in parallel
- **T050-T055** (GitHub Actions implementation) can be done in parallel with manifests

### Within Phase 6:
- **T086-T092** (Documentation) can be written in parallel
- **T093-T100** (Operational tooling) can be developed in parallel

---

## Success Criteria

Each phase must satisfy the following before proceeding to the next:

1. **Phase 1**: Directory structure created and documentation outline in place
2. **Phase 2**: Docker builds succeed, Terraform validates, GitHub Actions templates load
3. **Phase 3**: Terraform provisioning succeeds for all three environments without duplicates on re-run
4. **Phase 4**: Build workflow publishes images, deploy-dev succeeds, services are reachable
5. **Phase 5**: Promotion respects quality gates and approval requirements
6. **Phase 6**: Documentation is complete, end-to-end flow succeeds

---

## Acceptance Criteria

The implementation is complete when:

- ✓ All 104 tasks are marked complete
- ✓ Maintainers can provision a new environment in under 30 minutes (SC-001)
- ✓ 95%+ of deployment attempts succeed without manual intervention (SC-002)
- ✓ Operators can identify failed stages in under 10 minutes (SC-003)
- ✓ Same artifact promotes without rebuilding across all environments (SC-004)
- ✓ Blocked promotions clearly show the failing gate (SC-005)
- ✓ No secrets appear in committed source files (SC-006)
- ✓ All functional requirements (FR-001 through FR-015) are satisfied
- ✓ Documentation is complete and tested by a team member unfamiliar with the feature
