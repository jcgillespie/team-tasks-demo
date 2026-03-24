# Secret Rotation Runbook

## Purpose

Rotate Key Vault-backed secrets and validate application recovery without downtime.

## Scope

- SQL connection secret used by API App Service settings
- Any app settings using `@Microsoft.KeyVault(...)` references

## Rotation Steps

1. Create a new secret version in Key Vault for the target secret.
2. If needed, update the App Service setting reference to the new versioned URI.
3. Restart API and frontend apps (or slots) to refresh references.
4. Run smoke tests against `/api/tasks` and frontend root.

## Post-Rotation Validation

- `release.yml` smoke check succeeds in non-production first.
- API can read/write tasks with rotated secret.
- No startup configuration exceptions in app logs.

## Delivery Validation Task

For every production rotation, run this validation in order:

1. `infra-plan.yml` against target environment.
2. `infra-apply.yml` if configuration reference changed.
3. `release.yml` smoke validation against rotated configuration.
4. If any check fails, execute rollback runbook and restore previous secret version.
