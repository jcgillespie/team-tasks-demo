# Tasks: Infrastructure Delivery Automation

**Input**: Design documents from `/specs/001-iac-cicd-pipelines/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Test tasks are REQUIRED. Write tests first, confirm they fail, then implement to pass.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup

**Purpose**: Create the repository structure and shared entry points needed for delivery automation work.

- [ ] T001 Create infrastructure and operations documentation scaffolding in `infra/opentofu/README.md` and `docs/operations/README.md`
- [ ] T002 Create the delivery test workspace configuration in `tests/delivery/package.json`, `tests/delivery/tsconfig.json`, and `tests/delivery/vitest.config.ts`
- [ ] T003 [P] Create the release script command index in `scripts/release/README.md`

---

## Phase 2: Foundational

**Purpose**: Build the shared validation and configuration assets that block all user-story work.

**Critical**: No user story work should start until this phase is complete.

- [ ] T004 Create the workflow fixture catalog for infrastructure, CI, and release tests in `tests/delivery/src/shared/fixtureCatalog.ts`
- [ ] T005 [P] Create workflow YAML parsing and assertion helpers in `tests/delivery/src/shared/workflowTestUtils.ts` and `tests/delivery/src/shared/githubContext.ts`
- [ ] T006 [P] Create the shared release helper shell library in `scripts/release/common.sh` and `scripts/release/smoke-test.sh`
- [ ] T007 [P] Create shared OpenTofu provider, version, variable, and local definitions in `infra/opentofu/providers.tf`, `infra/opentofu/versions.tf`, `infra/opentofu/variables.tf`, and `infra/opentofu/locals.tf`
- [ ] T008 Create the GitHub Environment, OIDC, and branch-protection setup runbook in `docs/operations/environment-setup.md` and `docs/operations/pull-request-governance.md`

**Checkpoint**: Shared delivery tooling, workflow test harness, and environment conventions are ready.

---

## Phase 3: User Story 1 - Standardized Environment Provisioning (Priority: P1)

**Goal**: Provision dev, staging, and prod from one OpenTofu source of truth with isolated state, baseline monitoring, managed secrets, and plan/apply safeguards.

**Independent Test**: Run the infrastructure workflow for each environment and verify the plan artifact, apply boundary, state isolation, hosting resources, secret references, and monitoring resources are created with environment-specific configuration.

### Tests for User Story 1

- [ ] T009 [P] [US1] Create failing environment-root validation tests in `tests/delivery/src/infra/environment-roots.spec.ts`
- [ ] T010 [P] [US1] Create failing infrastructure workflow contract tests in `tests/delivery/src/infra/infra-workflows.spec.ts`

### Implementation for User Story 1

- [ ] T011 [P] [US1] Implement the shared naming and tagging module in `infra/opentofu/modules/platform_baseline/main.tf`, `infra/opentofu/modules/platform_baseline/variables.tf`, and `infra/opentofu/modules/platform_baseline/outputs.tf`
- [ ] T012 [P] [US1] Implement the App Service and monitoring module in `infra/opentofu/modules/app_service_stack/main.tf`, `infra/opentofu/modules/app_service_stack/variables.tf`, and `infra/opentofu/modules/app_service_stack/outputs.tf`
- [ ] T013 [P] [US1] Implement the data and secret management module in `infra/opentofu/modules/data_protection/main.tf`, `infra/opentofu/modules/data_protection/variables.tf`, and `infra/opentofu/modules/data_protection/outputs.tf`
- [ ] T014 [US1] Compose the dev environment root and backend configuration in `infra/opentofu/environments/dev/main.tf`, `infra/opentofu/environments/dev/variables.tf`, `infra/opentofu/environments/dev/outputs.tf`, `infra/opentofu/environments/dev/backend.hcl`, and `infra/opentofu/environments/dev/dev.tfvars`
- [ ] T015 [US1] Compose the staging environment root and backend configuration in `infra/opentofu/environments/staging/main.tf`, `infra/opentofu/environments/staging/variables.tf`, `infra/opentofu/environments/staging/outputs.tf`, `infra/opentofu/environments/staging/backend.hcl`, and `infra/opentofu/environments/staging/staging.tfvars`
- [ ] T016 [US1] Compose the prod environment root and backend configuration in `infra/opentofu/environments/prod/main.tf`, `infra/opentofu/environments/prod/variables.tf`, `infra/opentofu/environments/prod/outputs.tf`, `infra/opentofu/environments/prod/backend.hcl`, and `infra/opentofu/environments/prod/prod.tfvars`
- [ ] T017 [US1] Implement the infrastructure plan, apply, and drift-detection workflows in `.github/workflows/infra-plan.yml`, `.github/workflows/infra-apply.yml`, and `.github/workflows/drift-detection.yml`
- [ ] T018 [US1] Document provisioning flow, drift remediation, and environment outputs in `docs/operations/deployment.md` and `docs/operations/drift-response.md`

**Checkpoint**: A single infrastructure workflow can plan and provision each environment independently with auditable outputs and drift detection.

---

## Phase 4: User Story 2 - Safe Continuous Integration (Priority: P2)

**Goal**: Block merges with fast, automated quality and security validation on pull requests.

**Independent Test**: Open a pull request with intentional lint, test, and security failures and confirm the CI workflow fails with clear diagnostics; then fix the issues and confirm all required checks pass and publish reports.

### Tests for User Story 2

- [ ] T019 [P] [US2] Create failing PR CI regression tests in `tests/delivery/src/ci/ci-workflow.spec.ts`
- [ ] T020 [P] [US2] Create failing security and report-publication tests in `tests/delivery/src/ci/security-reporting.spec.ts`

### Implementation for User Story 2

- [ ] T021 [P] [US2] Implement the pull-request validation workflow in `.github/workflows/ci.yml`
- [ ] T022 [P] [US2] Configure dependency and secret-scanning baselines in `.github/dependabot.yml` and `.github/gitleaks.toml`
- [ ] T023 [US2] Implement reusable workflow summary and test-report scripts in `scripts/release/publish-test-report.sh` and `scripts/release/publish-workflow-summary.sh`
- [ ] T024 [US2] Define required checks and reviewer ownership in `.github/CODEOWNERS` and `docs/operations/pull-request-governance.md`
- [ ] T025 [US2] Update contributor guidance for CI expectations and failure handling in `README.md` and `specs/001-iac-cicd-pipelines/quickstart.md`

**Checkpoint**: Pull requests receive automated lint, build, test, and security feedback with report artifacts and merge-blocking governance.

---

## Phase 5: User Story 3 - Controlled Multi-Stage Release Promotion (Priority: P3)

**Goal**: Build once, promote the same artifact through dev, staging, and prod with approvals, health checks, and rollback.

**Independent Test**: Trigger a release from `main` or `release/*`, verify the same manifest and artifacts are promoted across environments with required approvals, and confirm rollback restores the last known-good version when a post-deploy check fails.

### Tests for User Story 3

- [ ] T026 [P] [US3] Create failing release manifest validation tests in `tests/delivery/src/release/release-manifest.spec.ts`
- [ ] T027 [P] [US3] Create failing promotion and rollback workflow tests in `tests/delivery/src/release/release-workflow.spec.ts`

### Implementation for User Story 3

- [ ] T028 [P] [US3] Implement manifest and checksum generation scripts in `scripts/release/create-manifest.sh` and `scripts/release/verify-checksums.sh`
- [ ] T029 [P] [US3] Implement App Service deploy and rollback scripts in `scripts/release/deploy-webapp.sh` and `scripts/release/rollback-webapp.sh`
- [ ] T030 [US3] Implement the multi-stage application release workflow in `.github/workflows/release.yml`
- [ ] T031 [US3] Add production slot deployment outputs and app-setting templates in `infra/opentofu/modules/app_service_stack/main.tf` and `infra/opentofu/modules/app_service_stack/outputs.tf`
- [ ] T032 [US3] Document staged promotion, rollback execution, and incident release decisions in `docs/operations/deployment.md`, `docs/operations/rollback.md`, and `docs/architecture.md`

**Checkpoint**: Releases are immutable, approval-gated, health-checked, and rollback-capable across all environments.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Close documentation, compliance, and operational validation gaps that span multiple stories.

- [ ] T033 [P] Update delivery compliance evidence in `specs/001-iac-cicd-pipelines/checklists/requirements.md`
- [ ] T034 [P] Validate quickstart and operator onboarding flow in `specs/001-iac-cicd-pipelines/quickstart.md` and `docs/operations/README.md`
- [ ] T035 Harden artifact retention, workflow permissions, and cleanup behavior in `.github/workflows/release.yml` and `.github/workflows/infra-apply.yml`
- [ ] T036 Run the full validation command matrix and capture operator notes in `docs/operations/README.md` and `README.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies; can start immediately.
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories.
- **Phase 3: User Story 1**: Depends on Phase 2.
- **Phase 4: User Story 2**: Depends on Phase 2.
- **Phase 5: User Story 3**: Depends on Phase 2 and reuses infrastructure assets from User Story 1.
- **Phase 6: Polish & Cross-Cutting Concerns**: Depends on the user stories selected for delivery.

### User Story Dependencies

- **US1**: No dependency on other user stories; it is the MVP foundation for hosted environments.
- **US2**: No dependency on other user stories; it can be delivered once shared delivery tooling is in place.
- **US3**: Depends on US1 for provisioned App Service and environment outputs; it should also consume CI outputs from US2 when available.

### Within Each User Story

- Tests must be written and observed failing before implementation tasks begin.
- Shared modules or scripts come before workflow wiring that consumes them.
- Environment composition follows reusable module completion.
- Documentation updates finish before the story is considered done.

### Parallel Opportunities

- `T003`, `T005`, `T006`, and `T007` can run in parallel once setup starts.
- `T009` and `T010` can run in parallel for US1.
- `T011`, `T012`, and `T013` can run in parallel for US1 after failing tests exist.
- `T019` and `T020` can run in parallel for US2.
- `T021` and `T022` can run in parallel for US2 after failing tests exist.
- `T026` and `T027` can run in parallel for US3.
- `T028` and `T029` can run in parallel for US3 after failing tests exist.
- `T033` and `T034` can run in parallel during polish.

---

## Parallel Example: User Story 1

```bash
# Launch both failing-first tests together
Task: T009 tests/delivery/src/infra/environment-roots.spec.ts
Task: T010 tests/delivery/src/infra/infra-workflows.spec.ts

# Launch reusable module work together after tests fail
Task: T011 infra/opentofu/modules/platform_baseline/main.tf
Task: T012 infra/opentofu/modules/app_service_stack/main.tf
Task: T013 infra/opentofu/modules/data_protection/main.tf
```

## Parallel Example: User Story 2

```bash
# Launch CI test coverage together
Task: T019 tests/delivery/src/ci/ci-workflow.spec.ts
Task: T020 tests/delivery/src/ci/security-reporting.spec.ts

# Launch independent implementation tasks together
Task: T021 .github/workflows/ci.yml
Task: T022 .github/dependabot.yml
```

## Parallel Example: User Story 3

```bash
# Launch release tests together
Task: T026 tests/delivery/src/release/release-manifest.spec.ts
Task: T027 tests/delivery/src/release/release-workflow.spec.ts

# Launch release helpers together after tests fail
Task: T028 scripts/release/create-manifest.sh
Task: T029 scripts/release/deploy-webapp.sh
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Validate environment provisioning, plan/apply boundaries, and drift detection before moving on.

### Incremental Delivery

1. Deliver US1 to establish reproducible environments.
2. Deliver US2 to protect ongoing changes with pull-request automation.
3. Deliver US3 to add release promotion and rollback on top of the stabilized foundation.
4. Finish with cross-cutting compliance, onboarding, and validation tasks.

### Parallel Team Strategy

1. One engineer completes setup and foundational work.
2. After Phase 2, one engineer can take US1 while another takes US2.
3. US3 starts once US1 environment outputs are stable and can proceed alongside final hardening of US2.

---

## Notes

- All task lines follow the required checklist format: checkbox, task ID, optional `[P]`, optional story label, and exact file path.
- Test-first sequencing is explicit for every user story.
- User Story 1 is the recommended MVP scope.