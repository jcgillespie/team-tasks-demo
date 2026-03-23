# Infrastructure Workflow Contract

## Purpose

Define the interface for OpenTofu validation, plan, apply, and drift-detection workflows that manage Azure infrastructure for `dev`, `staging`, and `prod`.

## Workflow Set

| Workflow | Trigger | Protected environment | Primary outcome |
|----------|---------|-----------------------|-----------------|
| `infra-plan.yml` | `pull_request`, `workflow_dispatch` | none | Saved plan artifacts and validation reports |
| `infra-apply.yml` | `workflow_dispatch`, `push` to protected release branches after approval | `dev`, `staging`, `prod` | Applied infrastructure changes from a saved plan |
| `drift-detection.yml` | `schedule`, `workflow_dispatch` | none | Alertable drift report without apply |

## Required Inputs

| Input | Type | Required | Description |
|-------|------|----------|-------------|
| `target_environment` | enum (`dev`, `staging`, `prod`) | yes | Environment root to initialize and plan/apply. |
| `working_directory` | string | yes | Environment directory under `/infra/opentofu/environments/{env}`. |
| `backend_config_path` | string | yes | Backend config file used for isolated remote state. |
| `var_file_path` | string | yes | Environment-specific variables or `.tfvars` file. |
| `plan_artifact_name` | string | plan/apply only | Name of the uploaded saved plan artifact. |
| `source_ref` | string | yes | Git ref or commit SHA associated with the run. |
| `apply_changes` | boolean | apply only | Explicit flag used to prevent accidental apply during plan runs. |

## Required Secrets and Identity

| Name | Source | Purpose |
|------|--------|---------|
| `AZURE_CLIENT_ID` | GitHub Environment secret | OIDC login principal per environment |
| `AZURE_TENANT_ID` | GitHub Environment secret | Tenant for federated login |
| `AZURE_SUBSCRIPTION_ID` | GitHub Environment secret | Subscription scope for the target environment |

Rules:

- Workflows must request GitHub Actions `id-token: write` permission and authenticate with `azure/login`.
- No publish profiles, storage keys, or long-lived service-principal secrets may be used.
- Azure RBAC must be scoped to the target environment resource group or narrower.

## Behavior Contract

1. Every run starts with `tofu fmt -check` and `tofu validate` before any plan or apply step.
2. `infra-plan.yml` must upload a saved plan artifact plus a human-readable summary for reviewers.
3. `infra-apply.yml` must consume a previously generated plan artifact and apply that exact plan rather than recalculating changes.
4. Production apply requires GitHub Environment approval before the apply job starts.
5. `drift-detection.yml` must never apply changes; it only reports non-empty plans.

## Outputs

| Output | Producer | Consumer | Description |
|--------|----------|----------|-------------|
| `plan_artifact` | `infra-plan.yml` | `infra-apply.yml` | Binary saved plan for exact apply |
| `plan_summary` | `infra-plan.yml` | reviewers | Markdown summary of changed resources |
| `terraform_outputs_json` | `infra-apply.yml` | `release.yml`, docs | Resource identifiers, hostnames, and secret-reference metadata |
| `drift_report` | `drift-detection.yml` | operators | Non-empty plan summary and remediation link |

## Failure Contract

- Missing backend configuration, missing OIDC secrets, or failed `tofu validate` must fail before planning.
- Any plan containing destructive production changes requires manual review and cannot be auto-applied.
- Apply failure blocks downstream application deployment for the same environment.
- Drift detection failure opens an operational follow-up but does not auto-remediate.

## Audit Requirements

- The plan summary must identify the target environment, source ref, and plan artifact name.
- Apply runs must record the source plan artifact and resulting output manifest.
- All workflow summaries must link to the corresponding runbook for apply failure or drift response.