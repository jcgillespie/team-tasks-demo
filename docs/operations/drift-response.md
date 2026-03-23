# Drift Response Runbook

## Purpose

Triage and remediate OpenTofu drift reported by `drift-detection.yml`.

## Entry Conditions

- Scheduled or manual drift run reports non-empty plan for an environment.

## Triage Steps

1. Download `drift-<env>` artifact and identify changed resources.
2. Determine whether drift is expected (approved hotfix) or unexpected.
3. For expected drift, backport changes into OpenTofu configuration.
4. For unexpected drift, create incident ticket and rollback manual changes if safe.

## Remediation Path

1. Open PR with corrective OpenTofu changes.
2. Validate with `infra-plan.yml`.
3. Apply via `infra-apply.yml` after environment approval.
4. Re-run `drift-detection.yml` to confirm clean state.

## Escalation

- Platform engineering on-call for production drift.
- Security team if drift includes identity or secret resources.
