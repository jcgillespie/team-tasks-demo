#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/common.sh"

require_command az
require_env WEBAPP_NAME
require_env RESOURCE_GROUP
require_env PACKAGE_PATH

slot_arg=()
if [[ -n "${WEBAPP_SLOT:-}" ]]; then
  slot_arg=(--slot "$WEBAPP_SLOT")
fi

log_info "Deploying $PACKAGE_PATH to $WEBAPP_NAME"
az webapp deploy \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --src-path "$PACKAGE_PATH" \
  "${slot_arg[@]}"
