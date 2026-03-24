# Application Release Contract

## Purpose

Define the release interface for building, publishing, promoting, validating, and rolling back the frontend and API application artifacts.

## Workflow Boundary

| Workflow | Trigger | Protected environment | Primary outcome |
|----------|---------|-----------------------|-----------------|
| `ci.yml` | `pull_request` | none | Fast validation for lint, test, build, and security checks |
| `release.yml` | `push` to `main` or `release/*`, `workflow_dispatch` | `dev`, `staging`, `prod` | Build once, deploy to dev, promote same artifact to staging and prod |

## Release Inputs

| Input | Type | Required | Description |
|-------|------|----------|-------------|
| `release_version` | string | yes | Immutable version label for the candidate release. |
| `source_commit_sha` | string | yes | Commit used to build the artifacts. |
| `frontend_package` | file/artifact | yes | Built React application package. |
| `api_package` | file/artifact | yes | Published ASP.NET Core package. |
| `deployment_manifest` | JSON artifact | yes | Metadata used for promotion, health checks, and rollback. |
| `target_environment` | enum (`dev`, `staging`, `prod`) | yes | Deployment stage. |

## Deployment Manifest Schema

| Field | Type | Description |
|-------|------|-------------|
| `releaseVersion` | string | Matches the workflow release input. |
| `sourceCommitSha` | string | Commit that produced the artifact set. |
| `frontendPackageName` | string | Artifact file name or package identifier. |
| `frontendSha256` | string | Frontend checksum. |
| `apiPackageName` | string | Artifact file name or package identifier. |
| `apiSha256` | string | API checksum. |
| `createdByRunId` | string | GitHub Actions run ID that built the artifacts. |
| `retentionUntil` | string (ISO 8601) | Minimum artifact retention deadline. |
| `rollbackVersion` | string or null | Last known-good release for the target environment. |

## Behavior Contract

1. `ci.yml` must run repository lint, build, tests, and security checks on pull requests.
2. `release.yml` must build the frontend and API once, calculate checksums, and publish a deployment manifest before any environment deployment begins.
3. The exact same artifact set must be promoted from `dev` to `staging` to `prod`; the workflow may not rebuild per environment.
4. Each environment deployment must execute health checks before being marked successful.
5. `staging` and `prod` promotions require GitHub Environment approval.

## Health and Rollback Contract

Required health checks after each deployment:

- Frontend returns `200` on the environment root URL.
- API returns `200` on `/api/tasks`.
- API startup validation confirms Key Vault-backed configuration is resolved.
- Smoke test confirms task list retrieval and task creation still succeed.

Rollback rules:

- A failed health check marks the promotion as failed.
- The workflow must redeploy the last known-good manifest or perform an App Service slot swap back to the previous live version.
- The rollback action must be recorded in the workflow summary with the recovered version identifier.

## Identity and Secret Rules

- Deployments authenticate to Azure using OIDC through `azure/login`.
- Application runtime secrets are stored in Azure Key Vault and referenced by App Service settings.
- Workflow-level secrets are limited to tenant, subscription, and client identifiers required for OIDC bootstrap.

## Outputs

| Output | Consumer | Description |
|--------|----------|-------------|
| `deployment_manifest` | rollback process, auditors | Immutable release metadata |
| `release_summary` | reviewers, approvers | Human-readable deployment result and next actions |
| `deployed_url_frontend` | operators | Environment frontend URL |
| `deployed_url_api` | operators | Environment API URL |
| `rollback_summary` | operators | Recovery details when rollback executes |

## Failure Contract

- Artifact build or checksum generation failure stops the release before deployment.
- Failed `dev` deployment blocks promotion to `staging` and `prod`.
- Approval timeout or rejection leaves the release in a blocked state without mutating higher environments.
- Production rollback must keep the previous known-good release reference intact for auditability.