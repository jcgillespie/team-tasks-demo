#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/common.sh"

require_env FRONTEND_PACKAGE
require_env API_PACKAGE
require_env FRONTEND_SHA256
require_env API_SHA256

actual_frontend_sha="$(shasum -a 256 "$FRONTEND_PACKAGE" | awk '{print $1}')"
actual_api_sha="$(shasum -a 256 "$API_PACKAGE" | awk '{print $1}')"

[[ "$actual_frontend_sha" == "$FRONTEND_SHA256" ]] || {
  log_error "Frontend checksum mismatch"
  exit 1
}

[[ "$actual_api_sha" == "$API_SHA256" ]] || {
  log_error "API checksum mismatch"
  exit 1
}

log_info "Checksums verified"
