#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/common.sh"

require_command az
require_env WEBAPP_NAME
require_env RESOURCE_GROUP
require_env SOURCE_SLOT
require_env TARGET_SLOT

log_info "Swapping slots to rollback deployment"
az webapp deployment slot swap \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --slot "$SOURCE_SLOT" \
  --target-slot "$TARGET_SLOT"
