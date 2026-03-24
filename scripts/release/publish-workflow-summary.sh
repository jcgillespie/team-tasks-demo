#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${GITHUB_STEP_SUMMARY:-}" ]]; then
  echo "GITHUB_STEP_SUMMARY not set; skipping workflow summary"
  exit 0
fi

{
  echo "## Workflow Summary"
  echo
  echo "- CI completed quality gate checks"
  echo "- Security scanning completed"
  echo "- Required reports published"
} >> "$GITHUB_STEP_SUMMARY"
