#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/common.sh"

require_env FRONTEND_URL
require_env API_URL

log_info "Checking frontend health at $FRONTEND_URL"
curl --fail --silent --show-error "$FRONTEND_URL" >/dev/null

log_info "Checking API health at $API_URL/api/tasks"
curl --fail --silent --show-error "$API_URL/api/tasks" >/dev/null

log_info "Smoke checks passed"
