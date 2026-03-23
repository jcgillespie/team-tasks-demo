#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/common.sh"

require_env RELEASE_VERSION
require_env SOURCE_COMMIT_SHA
require_env FRONTEND_PACKAGE
require_env API_PACKAGE
require_env OUTPUT_PATH

frontend_sha="$(shasum -a 256 "$FRONTEND_PACKAGE" | awk '{print $1}')"
api_sha="$(shasum -a 256 "$API_PACKAGE" | awk '{print $1}')"

cat > "$OUTPUT_PATH" <<JSON
{
  "releaseVersion": "$RELEASE_VERSION",
  "sourceCommitSha": "$SOURCE_COMMIT_SHA",
  "frontendPackageName": "$(basename "$FRONTEND_PACKAGE")",
  "frontendSha256": "$frontend_sha",
  "apiPackageName": "$(basename "$API_PACKAGE")",
  "apiSha256": "$api_sha",
  "createdByRunId": "${GITHUB_RUN_ID:-local}",
  "retentionUntil": "${RETENTION_UNTIL:-}",
  "rollbackVersion": "${ROLLBACK_VERSION:-}"
}
JSON

log_info "Manifest written to $OUTPUT_PATH"
