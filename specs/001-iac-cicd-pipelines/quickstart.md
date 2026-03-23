# Quickstart

## Goal

Stand up a reproducible Azure delivery foundation for the Team Tasks application using OpenTofu and GitHub Actions with isolated `dev`, `staging`, and `prod` environments.

## Planned Tooling

- Node.js 22+
- `pnpm` 10+
- .NET SDK 10
- OpenTofu 1.8+
- Azure CLI with access to configure Microsoft Entra federated credentials
- GitHub repository admin access for environments, branch protection, and required checks

## Local Validation Baseline

Run the existing application quality gates locally before changing infrastructure or pipelines:

```bash
dotnet restore TeamTasks.slnx
dotnet build TeamTasks.slnx
dotnet test TeamTasks.slnx

cd client
pnpm install
pnpm lint
pnpm build
pnpm test
```

## Planned Repository Additions

```text
.github/workflows/
infra/opentofu/
docs/operations/
```

Expected OpenTofu layout:

```text
infra/opentofu/
├── backend/
├── modules/
└── environments/
    ├── dev/
    ├── staging/
    └── prod/
```

## Environment Model

- `dev`: first deployment target for all infrastructure and application releases
- `staging`: approval-gated promotion environment for validation before production
- `prod`: approval-gated production environment with rollback-capable deployment flow

Each environment receives its own Azure resource group, Key Vault, monitoring resources, SQL database, and isolated OpenTofu state backend configuration.

## Planned GitHub Actions Workflow Map

1. `ci.yml`
   - Trigger: pull requests
   - Runs lint, tests, builds, and security checks
2. `infra-plan.yml`
   - Trigger: pull requests and manual dispatch
   - Runs `tofu fmt -check`, `tofu validate`, and `tofu plan`
3. `infra-apply.yml`
   - Trigger: manual dispatch or protected branch flow
   - Applies a previously saved plan after environment approval
4. `release.yml`
   - Trigger: `main`, `release/*`, or manual dispatch
   - Builds once, deploys to `dev`, then promotes the same artifact through `staging` and `prod`
5. `drift-detection.yml`
   - Trigger: nightly schedule and manual dispatch
   - Detects drift without applying changes

## OIDC Setup Summary

1. Create GitHub Environments named `dev`, `staging`, and `prod`.
2. Create Microsoft Entra federated credentials scoped to each GitHub Environment.
3. Store only these GitHub Environment secrets:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_SUBSCRIPTION_ID`
4. Grant Azure RBAC at the target environment resource group scope or narrower.

## Release and Rollback Flow

1. Merge a validated change to `main` or cut a `release/*` branch.
2. Build frontend and API artifacts once and publish a deployment manifest.
3. Deploy to `dev` and run smoke checks.
4. After approval, promote the same artifacts to `staging`.
5. After approval, promote the same artifacts to `prod`.
6. If health checks fail, redeploy the last known-good manifest or swap the App Service slot back.

## Required Runbooks

- Deployment execution
- Rollback execution
- Drift triage and remediation
- Secret rotation and validation

These runbooks should live under `/docs/operations/` and be updated in the same pull request as the workflows they describe.