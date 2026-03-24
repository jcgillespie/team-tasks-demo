# Release Script Command Index

This directory contains helper scripts used by CI and release workflows.

## Shared Helpers

- `common.sh`: strict shell helpers, logging, and guard functions
- `smoke-test.sh`: deployment smoke validation against frontend and API endpoints

## Release Artifact Helpers

- `create-manifest.sh`: create immutable release metadata and checksums
- `verify-checksums.sh`: verify artifact integrity before deployment

## Deployment Helpers

- `deploy-webapp.sh`: deploy build artifacts to Azure App Service
- `rollback-webapp.sh`: rollback to the previous known-good version

## CI Reporting Helpers

- `publish-test-report.sh`: upload test evidence and report links
- `publish-workflow-summary.sh`: write standardized workflow summary markdown
