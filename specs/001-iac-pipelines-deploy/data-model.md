# Data Model: Deployment IaC and Pipelines

## Deployment Environment

- Purpose: Represents one of the supported runtime targets: development, stage, or production.
- Fields:
  - `name`: stable environment identifier
  - `subscriptionScope`: Azure subscription or resource-group scope reference
  - `clusterNamespace`: Kubernetes namespace or equivalent logical boundary
  - `domainHost`: externally reachable hostname for the environment
  - `approvalRequired`: whether promotion into the environment requires approval
  - `qualityGateProfile`: reference to the configured quality-gate input set
  - `secretReferences`: list of secure configuration references required for deployment
- Relationships:
  - Owns one or more deployment targets
  - Receives promoted release artifacts
  - Uses one quality-gate profile per promotion boundary
- Validation Rules:
  - `name` must be one of `development`, `stage`, or `production` for v1
  - `domainHost` must be unique per environment
  - `approvalRequired` must be true for production

## Infrastructure Definition

- Purpose: Represents the version-controlled Terraform composition for shared and environment-specific Azure resources.
- Fields:
  - `moduleSet`: referenced Terraform modules
  - `environmentValues`: per-environment variable set
  - `resourceInventory`: expected Azure resource categories created by the plan
  - `stateBoundary`: Terraform state isolation boundary
  - `revision`: source-control revision associated with the definition
- Relationships:
  - Provisions deployment environments
  - Supplies outputs consumed by pipeline runs and Kubernetes deployments
- Validation Rules:
  - Shared modules must not embed environment-specific secrets
  - Environment values must be override-only and not duplicate shared definitions unnecessarily

## Release Artifact

- Purpose: Represents the deployable application version promoted through environments.
- Fields:
  - `artifactId`: immutable release identifier
  - `sourceRevision`: git commit or tag reference
  - `clientImage`: frontend container image reference
  - `apiImage`: API container image reference
  - `buildMetadata`: test, lint, and build result summary
- Relationships:
  - Produced by a pipeline run
  - Deployed to one or more environments
- Validation Rules:
  - Artifact references must be immutable and traceable to a single source revision
  - Both client and API images must be present for a promotable release

## Pipeline Run

- Purpose: Represents one execution of the CI/CD workflow.
- Fields:
  - `runId`: unique workflow execution identifier
  - `triggerType`: push, pull request, manual dispatch, or promotion event
  - `targetStage`: validation, build, provision, deploy, or promote
  - `status`: pending, running, failed, succeeded, blocked
  - `startedAt`: execution start time
  - `completedAt`: execution completion time
  - `failureContext`: stage and message recorded on failure
- Relationships:
  - Produces release artifacts
  - Evaluates quality gates
  - Applies infrastructure definitions
- Validation Rules:
  - `failureContext` is required when `status` is failed or blocked
  - Promotion runs must reference the artifact being promoted

## Quality Gate

- Purpose: Represents a configurable promotion checkpoint evaluated before movement to the next environment.
- Fields:
  - `gateName`: human-readable gate identifier
  - `appliesToTransition`: source-to-target environment transition
  - `evaluationStatus`: passed, failed, skipped, unknown
  - `evaluationEvidence`: pointer to pipeline evidence used by the gate
  - `failureReason`: explanatory message when blocking occurs
- Relationships:
  - Attached to promotion boundaries
  - Evaluated during pipeline runs
- Validation Rules:
  - A gate must identify exactly one environment transition
  - `failureReason` is required when `evaluationStatus` is failed

## Protected Secret

- Purpose: Represents a deployment-time sensitive value referenced securely by automation.
- Fields:
  - `secretName`: logical identifier
  - `provider`: secure storage provider reference
  - `consumedBy`: Terraform, workflow, or Kubernetes workload
  - `rotationOwner`: responsible operational owner
- Relationships:
  - Referenced by infrastructure definitions and pipeline runs
  - Consumed by deployment environments
- Validation Rules:
  - Secret values must never be committed to source
  - Each secret must identify a consumer and storage provider