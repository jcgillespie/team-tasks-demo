# Operations Runbooks

This directory contains runbooks for delivery automation and production operations.

## Runbooks

- `environment-setup.md`: GitHub Environments, OIDC, and RBAC setup
- `deployment.md`: infrastructure and app deployment flow
- `rollback.md`: release rollback process
- `drift-response.md`: handling infrastructure drift
- `secret-rotation.md`: rotating Key Vault and app configuration secrets
- `pull-request-governance.md`: required checks and review policy

## Operator Validation Matrix

Minimum command matrix for release readiness:

```bash
dotnet build TeamTasks.slnx
dotnet test TeamTasks.slnx
cd client && pnpm lint && pnpm build && pnpm test
```

Infrastructure validation:

```bash
cd infra/opentofu/environments/<env>
tofu fmt -check
tofu validate
tofu plan -var-file=<env>.tfvars
```

## Operator Onboarding Flow

1. Read `environment-setup.md` and configure GitHub Environments/OIDC.
2. Read `pull-request-governance.md` for merge gates and evidence expectations.
3. Run the validation matrix above in a clean branch.
4. Dry-run `infra-plan.yml` and `release.yml` via workflow dispatch.
5. Practice rollback using `rollback.md` and secret rotation using `secret-rotation.md`.
