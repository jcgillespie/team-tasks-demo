# Environment Setup Runbook

## Purpose

Configure GitHub Environments, Azure OIDC federation, and branch protections for delivery automation.

## Prerequisites

- Repository admin access
- Azure subscription access with RBAC assignment rights
- Existing Microsoft Entra app registration for GitHub OIDC

## Steps

1. Create GitHub Environments named `dev`, `staging`, and `prod`.
2. Add required environment secrets to each environment:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID`
   - `AZURE_SUBSCRIPTION_ID`
3. Require reviewers for `staging` and `prod` environments.
4. In Microsoft Entra, add a federated credential for each GitHub Environment subject.
5. Assign least-privilege RBAC at resource group scope for each environment.
6. Configure branch protection to require CI and delivery checks before merge.

## Validation

- Run `infra-plan.yml` for each environment and confirm successful OIDC login.
- Verify a protected environment requires approval before job execution.
