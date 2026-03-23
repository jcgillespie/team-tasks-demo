#!/usr/bin/env bash
set -euo pipefail

log_info() {
  echo "[INFO] $*"
}

log_warn() {
  echo "[WARN] $*"
}

log_error() {
  echo "[ERROR] $*" >&2
}

require_env() {
  local key="$1"
  if [[ -z "${!key:-}" ]]; then
    log_error "Missing required environment variable: $key"
    exit 1
  fi
}

require_command() {
  local cmd="$1"
  if ! command -v "$cmd" >/dev/null 2>&1; then
    log_error "Required command is not available: $cmd"
    exit 1
  fi
}
