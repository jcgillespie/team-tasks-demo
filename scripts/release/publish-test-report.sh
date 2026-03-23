#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${GITHUB_STEP_SUMMARY:-}" ]]; then
  echo "GITHUB_STEP_SUMMARY not set; skipping report publishing"
  exit 0
fi

{
  echo "## Test Evidence"
  echo
  echo "- Client: lint, build, and test executed"
  echo "- Server: dotnet build/test executed"
} >> "$GITHUB_STEP_SUMMARY"
