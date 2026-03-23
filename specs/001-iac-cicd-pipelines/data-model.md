# Data Model

## Environment Blueprint

**Purpose**: Canonical declaration of one deployment stage and its Azure footprint.

| Field | Type | Description |
|-------|------|-------------|
| `environment_name` | enum (`dev`, `staging`, `prod`) | Lifecycle stage identifier. |
| `location` | string | Azure region for the stage. |
| `resource_group_name` | string | Stage-specific resource group. |
| `tags` | map<string,string> | Required governance and ownership metadata. |
| `app_service_plan_name` | string | Shared compute plan for frontend and API web apps. |
| `frontend_app_name` | string | Public web app hosting the built React site. |
| `api_app_name` | string | Web app hosting the ASP.NET Core API. |
| `production_slot_enabled` | boolean | Whether the stage uses a staging slot for safe swap rollout. |
| `sql_server_name` | string | Azure SQL logical server name. |
| `sql_database_name` | string | Stage-local application database. |
| `key_vault_name` | string | Stage-local secret store. |
| `application_insights_name` | string | Telemetry resource for app monitoring. |
| `log_analytics_workspace_name` | string | Central log store for the stage. |
| `state_container_name` | string | Azure Storage container used for OpenTofu state. |
| `allowed_origins` | list<string> | Frontend origin list allowed by the API. |

**Validation rules**

- `environment_name` must be one of `dev`, `staging`, or `prod`.
- Resource names must follow a single naming convention derived from app name, environment, region, and resource type.
- `production_slot_enabled` is required for `prod` and optional for `dev` and `staging`.
- `allowed_origins` must include the stage frontend hostname and must not include wildcard production origins.

## Infrastructure Module Instance

**Purpose**: Reusable OpenTofu module invocation bound to a specific environment.

| Field | Type | Description |
|-------|------|-------------|
| `module_name` | string | Logical module identifier such as `app_service`, `sql`, or `monitoring`. |
| `source_path` | string | Module path under `/infra/opentofu/modules`. |
| `environment_name` | enum | Stage consuming the module. |
| `input_variables` | map<string,any> | Concrete values supplied by the environment root. |
| `outputs` | map<string,any> | Values exported for downstream modules or workflows. |
| `depends_on_modules` | list<string> | Ordered dependencies for safe provisioning. |

**Validation rules**

- All environment roots must use the same module set unless a deviation is explicitly documented.
- Output names must be stable and human-readable because pipelines consume them.
- Cross-environment dependencies are not allowed.

## Secret Reference

**Purpose**: Mapping between an application setting and the Key Vault secret that backs it.

| Field | Type | Description |
|-------|------|-------------|
| `setting_name` | string | App setting or connection string name exposed to the workload. |
| `vault_name` | string | Source Key Vault. |
| `secret_name` | string | Secret object resolved at runtime. |
| `secret_version_pinned` | boolean | Whether a version-specific reference is used. |
| `consumer` | enum (`frontend`, `api`, `pipeline`) | Consumer of the secret. |
| `access_principal` | string | Managed identity or OIDC principal that resolves it. |

**Validation rules**

- Plaintext secret values are never stored in source control or workflow YAML.
- `pipeline` consumers may read only bootstrap or deployment-time secrets that cannot be injected as resource references.
- Production secret references must be stage-local and must not target development vaults.

## Release Artifact

**Purpose**: Immutable package set promoted between environments.

| Field | Type | Description |
|-------|------|-------------|
| `release_version` | string | Human-readable version label derived from tag, run number, and commit. |
| `source_commit_sha` | string | Commit used to build the artifact. |
| `frontend_package_path` | string | Published frontend package or archive path. |
| `api_package_path` | string | Published API package or archive path. |
| `frontend_sha256` | string | Integrity checksum for the frontend package. |
| `api_sha256` | string | Integrity checksum for the API package. |
| `manifest_path` | string | Path to deployment manifest consumed by release automation. |
| `created_by_run_id` | string | GitHub Actions run identifier that built the package set. |
| `retention_until` | datetime | Minimum retention deadline for rollback support. |

**Validation rules**

- Checksums must be recorded before any deployment starts.
- A promoted artifact cannot be rebuilt or mutated in later stages.
- `retention_until` must satisfy the documented rollback retention policy.

## Pipeline Run

**Purpose**: Auditable execution record for validation, provisioning, deployment, or drift detection.

| Field | Type | Description |
|-------|------|-------------|
| `run_id` | string | GitHub Actions run identifier. |
| `workflow_name` | enum (`ci`, `infra-plan`, `infra-apply`, `release`, `drift-detection`) | Workflow category. |
| `trigger_type` | enum (`pull_request`, `push`, `workflow_dispatch`, `schedule`) | How the run started. |
| `target_environment` | enum or null | Stage targeted by the run, if applicable. |
| `source_ref` | string | Branch, tag, or commit ref. |
| `status` | enum (`queued`, `running`, `failed`, `succeeded`, `cancelled`) | Run outcome. |
| `report_artifacts` | list<string> | Paths or identifiers for logs, plans, and test reports. |
| `started_at` | datetime | Run start timestamp. |
| `completed_at` | datetime | Run completion timestamp. |

**Validation rules**

- `infra-apply` runs must reference a saved plan artifact from a successful `infra-plan` run.
- `release` runs must reference a previously created `ReleaseArtifact`.
- `prod` runs require environment approval metadata before status can transition to `running`.

## Promotion Record

**Purpose**: Captures a release artifact moving through environments with governance and rollback evidence.

| Field | Type | Description |
|-------|------|-------------|
| `promotion_id` | string | Stable identifier for one release progression. |
| `release_version` | string | Artifact version being promoted. |
| `from_environment` | enum or null | Prior stage, null for first deployment to dev. |
| `to_environment` | enum (`dev`, `staging`, `prod`) | Target stage. |
| `approval_environment` | string | GitHub Environment that protects the step. |
| `approver_handles` | list<string> | Users or teams that approved the promotion. |
| `health_check_result` | enum (`pending`, `passed`, `failed`) | Outcome of post-deploy validation. |
| `rollback_target_version` | string or null | Last known-good version if rollback is needed. |
| `result` | enum (`pending`, `promoted`, `rolled_back`, `blocked`) | Final promotion state. |

**State transitions**

- `pending -> promoted` when deployment and health checks pass.
- `pending -> blocked` when approvals or required checks do not pass.
- `pending -> rolled_back` when deployment succeeds technically but health checks fail and rollback is executed.

## Operational Runbook

**Purpose**: Documented human procedure required by governance and incident response.

| Field | Type | Description |
|-------|------|-------------|
| `runbook_name` | enum (`deployment`, `rollback`, `drift-response`, `secret-rotation`) | Procedure type. |
| `owner` | string | Team or role accountable for maintenance. |
| `entry_conditions` | list<string> | Preconditions for executing the runbook. |
| `steps` | list<string> | Ordered human actions. |
| `validation_checks` | list<string> | What confirms success or safe termination. |
| `escalation_path` | list<string> | Contacts or teams to involve when the runbook fails. |

**Validation rules**

- Every runbook must identify its owner and last review date.
- Rollback and drift-response runbooks must include explicit stop conditions to prevent unsafe repeated actions.

## Relationships

- One `EnvironmentBlueprint` owns many `InfrastructureModuleInstance` records.
- One `EnvironmentBlueprint` owns many `SecretReference` records.
- One `ReleaseArtifact` participates in many `PromotionRecord` records.
- One `PipelineRun` may create one `ReleaseArtifact` or one saved infrastructure plan.
- One `PromotionRecord` must reference exactly one `ReleaseArtifact` and one `PipelineRun`.
- One `OperationalRunbook` supports one or more pipeline or promotion scenarios.