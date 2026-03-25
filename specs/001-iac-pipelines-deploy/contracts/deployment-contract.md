# Deployment Contract

## Purpose

Define the external operational contract between repository maintainers, GitHub Actions workflows, Terraform infrastructure assets, and AKS deployment targets for the Team Tasks application.

## Contract 1: Infrastructure Provisioning Input Contract

- Consumer: Maintainer or automation invoking Terraform
- Inputs:
  - Environment identifier: `development`, `stage`, or `production`
  - Azure location and naming prefix
  - Terraform backend configuration
  - Secure references for secrets and identity bindings
- Guarantees:
  - Provisioning is repeatable for the selected environment
  - Outputs include cluster access metadata, registry coordinates, and secret/provider references required by deployment automation
  - Re-running with unchanged inputs results in no unintended duplicate resources

## Contract 2: CI Validation Contract

- Consumer: Pull request and merge workflows
- Inputs:
  - Source revision
  - Backend test commands
  - Frontend lint and test commands
- Guarantees:
  - Validation result is surfaced as pass or fail
  - Failure output identifies the stage that failed
  - Promotion is not attempted from a revision that fails validation

## Contract 3: Artifact Publication Contract

- Consumer: Build workflow
- Inputs:
  - Source revision
  - Container build definitions for client and API
  - Registry authentication context
- Guarantees:
  - Published images are immutable and traceable to the source revision
  - Both application components are version-aligned in the resulting release artifact

## Contract 4: Environment Promotion Contract

- Consumer: Promotion workflow
- Inputs:
  - Release artifact identifier
  - Source and target environments
  - Quality-gate evaluation results
  - Approval state where required
- Guarantees:
  - The same artifact is promoted without rebuilding between environments
  - Promotion is blocked if configured quality gates fail
  - Promotion to production is blocked until approval is granted
  - Blocked promotions expose the gate or approval reason clearly

## Contract 5: Runtime Configuration Contract

- Consumer: AKS deployments for frontend and API workloads
- Inputs:
  - Environment-scoped non-secret configuration
  - Secure secret references
  - Image references for the release artifact
- Guarantees:
  - Environment-specific values can change without changing the shared deployment definition
  - Secret values are resolved securely outside committed source
  - Runtime configuration remains consistent with the selected environment and artifact version