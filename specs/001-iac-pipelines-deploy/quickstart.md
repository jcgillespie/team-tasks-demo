# Quickstart: Deployment IaC and Pipelines

## Prerequisites

- Terraform installed locally for infrastructure validation
- Docker available for local image build verification
- .NET 10 SDK installed
- Node.js and `pnpm` installed
- Azure subscription access with permission to create AKS, ACR, networking, identity, and secret resources
- GitHub repository admin access to configure Actions environments, approvals, and OIDC federation

## Repository Preparation

1. Restore and validate the application locally.
2. Confirm backend and frontend tests pass.
3. Confirm both workloads can build into production-ready container images.

## Infrastructure Workflow

1. Set environment-specific Terraform variables for development, stage, and production.
2. Run Terraform formatting and validation.
3. Review the Terraform plan for the target environment.
4. Apply infrastructure for the target environment.
5. Capture outputs required by GitHub Actions and Kubernetes deployment configuration.

## GitHub Actions Setup

1. Create GitHub environments for `development`, `stage`, and `production`.
2. Configure required reviewers for the `production` environment.
3. Configure OIDC federation between GitHub Actions and Azure.
4. Add repository or environment secrets only for values that cannot be sourced dynamically.
5. Configure workflow variables for Terraform state, image naming, and environment targeting.

## Delivery Flow

1. Run validation workflow on pull requests to execute linting and tests.
2. Run build workflow on qualifying merges to build and publish frontend and API images.
3. Deploy automatically to development after successful build and infrastructure readiness checks.
4. Promote the same release to stage after configured quality gates pass.
5. Promote the same release to production only after stage gates pass and production approval is granted.

## Verification

1. Confirm both workloads are running in AKS for the target environment.
2. Confirm ingress routes frontend and API traffic correctly.
3. Confirm blocked promotions report the failed quality gate.
4. Confirm rollback or redeploy uses the same immutable artifact references.

## Documentation Updates Required During Implementation

- Update repository README with deployment prerequisites and commands.
- Update `docs/architecture.md` with AKS, Terraform, and GitHub Actions architecture details.
- Document environment bootstrap, promotion, and operational troubleshooting steps.