#!/usr/bin/env bash
# environment-status.sh
#
# Displays the current status of a Team Tasks deployment environment.
# Usage:
#   environment-status.sh <environment>
#
# Output:
#   - Pod status for API and client
#   - Currently deployed image tags
#   - Ingress external IP
#   - Recent events

set -euo pipefail

ENVIRONMENT="${1:-}"

if [[ -z "${ENVIRONMENT}" ]]; then
  echo "Usage: $0 <environment>" >&2
  echo "  environment: development | stage | production" >&2
  exit 1
fi

NAMESPACE="${ENVIRONMENT}"

echo "========================================"
echo "  Team Tasks — Environment: ${NAMESPACE}"
echo "========================================"
echo ""

echo "--- Pods ---"
kubectl get pods -n "${NAMESPACE}" \
  -l 'app in (teamtasks-api,teamtasks-client)' \
  -o wide 2>/dev/null || echo "  (namespace not found or no pods)"

echo ""
echo "--- Deployed Images ---"
kubectl get pods -n "${NAMESPACE}" \
  -l 'app in (teamtasks-api,teamtasks-client)' \
  -o jsonpath='{range .items[*]}{.metadata.name}{"\t"}{.spec.containers[0].image}{"\n"}{end}' 2>/dev/null \
  || echo "  (could not retrieve)"

echo ""
echo "--- Services ---"
kubectl get services -n "${NAMESPACE}" \
  -l 'app in (teamtasks-api,teamtasks-client)' 2>/dev/null \
  || echo "  (none found)"

echo ""
echo "--- Ingress ---"
kubectl get ingress -n "${NAMESPACE}" 2>/dev/null \
  || echo "  (none found)"

echo ""
echo "--- Recent Events (warnings only) ---"
kubectl get events -n "${NAMESPACE}" \
  --field-selector=type=Warning \
  --sort-by='.lastTimestamp' 2>/dev/null | tail -10 \
  || echo "  (no warnings)"

echo ""
echo "--- Persistent Volumes ---"
kubectl get pvc -n "${NAMESPACE}" 2>/dev/null \
  || echo "  (none)"

echo ""
echo "========================================"
echo "  End of status report"
echo "========================================"
