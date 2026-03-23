# Deployment Runbook

## Purpose

Execute infrastructure and application deployments across `dev`, `staging`, and `prod` with saved plan and immutable artifact safeguards.

## Infrastructure Provisioning Flow

1. Run `infra-plan.yml` for target environment.
2. Review plan artifact and summary.
3. Approve and execute `infra-apply.yml` using the exact saved plan artifact.
4. Collect `outputs.json` and verify hostnames, Key Vault URI, and SQL metadata.

## Application Release Flow

1. Trigger `release.yml` from `main` or `release/*`.
2. Build frontend and API once and publish manifest.
3. Deploy to `dev` and run smoke tests.
4. Promote same artifacts to `staging` after approval.
5. Promote same artifacts to `prod` after approval.

## Post-Deployment Validation

- Frontend root returns `200`.
- API endpoint `/api/tasks` returns `200`.
- Hosted API resolves `ConnectionStrings:DefaultConnection`.
- Smoke test script succeeds.

## Incident Release Decisions

- If health checks fail in `dev`, stop promotion and fix forward.
- If health checks fail in `staging`, block production promotion and open incident review.
- If health checks fail in `prod`, execute rollback runbook immediately and record recovered version.

## Rotated Secret Recovery Validation

After a production secret rotation:

1. Run smoke checks against frontend and API endpoints.
2. Trigger `release.yml` in dry-run mode (no new code changes) to verify hosted configuration recovery.
3. If failures persist, restore prior secret version and execute rollback runbook.

## Outputs to Capture

- `infra-plan-<env>` and `infra-outputs-<env>` artifacts
- deployment manifest JSON
- workflow summary markdown links
