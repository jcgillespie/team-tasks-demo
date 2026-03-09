#!/usr/bin/env bash

# Do not source this script; run it as ./scripts/dev.sh
if { [ -n "${ZSH_EVAL_CONTEXT:-}" ] && [[ "${ZSH_EVAL_CONTEXT}" == *:file ]]; } ||
	 [ "${BASH_SOURCE[0]:-$0}" != "$0" ]; then
	echo "Run this script directly: ./scripts/dev.sh" >&2
	return 1 2>/dev/null || exit 1
fi

set -euo pipefail

# Starts backend API and frontend dev server in two terminal sessions.
SCRIPT_PATH="${BASH_SOURCE[0]:-$0}"
SCRIPT_DIR="$(cd -- "$(dirname -- "$SCRIPT_PATH")" && pwd)"
ROOT_DIR="$(cd -- "$SCRIPT_DIR/.." && pwd)"

echo "Starting backend API at http://localhost:5276"
osascript -e "tell application \"Terminal\" to do script \"cd '$ROOT_DIR/server/TeamTasks.Api' && dotnet run\""

echo "Starting frontend app at http://localhost:5173"
osascript -e "tell application \"Terminal\" to do script \"echo 'Waiting for API on http://localhost:5276/api/tasks ...' && until curl -fsS http://localhost:5276/api/tasks >/dev/null 2>&1; do sleep 1; done && echo 'API is up, starting frontend.' && cd '$ROOT_DIR/client' && pnpm dev\""

echo "Both processes started in new Terminal windows."
