# Rollback Runbook

## Purpose

Restore the last known-good release when post-deployment checks fail.

## Preconditions

- Failed deployment identified in `release.yml`.
- Previous manifest version is available.

## Rollback Options

1. Slot swap rollback:
   - swap `staging` slot back to `production` for API or frontend
2. Manifest rollback:
   - redeploy previous release artifacts referenced by manifest

## Slot Rollback Command

```bash
WEBAPP_NAME=<app-name> \
RESOURCE_GROUP=<rg-name> \
SOURCE_SLOT=production \
TARGET_SLOT=staging \
./scripts/release/rollback-webapp.sh
```

## Validation

- Frontend and API health checks return `200`.
- Incident notes include failed release ID and recovered release ID.
