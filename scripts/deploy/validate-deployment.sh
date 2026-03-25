#!/usr/bin/env bash
# validate-deployment.sh
#
# Validates that a deployment is healthy after a kubectl apply/rollout.
# Usage:
#   validate-deployment.sh <environment> <artifact-id>
#
# Exits 0 on success, 1 on failure.

set -euo pipefail

ENVIRONMENT="${1:-}"
ARTIFACT_ID="${2:-}"

if [[ -z "${ENVIRONMENT}" || -z "${ARTIFACT_ID}" ]]; then
  echo "Usage: $0 <environment> <artifact-id>" >&2
  exit 1
fi

NAMESPACE="${ENVIRONMENT}"

echo "🔍 Validating deployment in namespace: ${NAMESPACE}"
echo "   Artifact: ${ARTIFACT_ID}"
echo ""

FAILED=0

# Check API deployment rollout
echo "Checking teamtasks-api rollout..."
if kubectl rollout status deployment/teamtasks-api -n "${NAMESPACE}" --timeout=2m 2>&1; then
  echo "  ✅ teamtasks-api: rollout complete"
else
  echo "  ❌ teamtasks-api: rollout failed or timed out"
  FAILED=1
fi

# Check client deployment rollout
echo "Checking teamtasks-client rollout..."
if kubectl rollout status deployment/teamtasks-client -n "${NAMESPACE}" --timeout=2m 2>&1; then
  echo "  ✅ teamtasks-client: rollout complete"
else
  echo "  ❌ teamtasks-client: rollout failed or timed out"
  FAILED=1
fi

# Check pods are running
echo "Checking pod status..."
NOT_RUNNING=$(kubectl get pods -n "${NAMESPACE}" \
  -l 'app in (teamtasks-api,teamtasks-client)' \
  --field-selector='status.phase!=Running' \
  -o name 2>/dev/null | wc -l | xargs)

if [[ "${NOT_RUNNING}" -eq 0 ]]; then
  echo "  ✅ All pods are Running"
else
  echo "  ❌ ${NOT_RUNNING} pod(s) are not Running"
  kubectl get pods -n "${NAMESPACE}" -l 'app in (teamtasks-api,teamtasks-client)'
  FAILED=1
fi

# List deployed images for audit trail
echo ""
echo "Deployed images:"
kubectl get pods -n "${NAMESPACE}" \
  -l 'app in (teamtasks-api,teamtasks-client)' \
  -o jsonpath='{range .items[*]}{.metadata.name}{"\t"}{.spec.containers[0].image}{"\n"}{end}' 2>/dev/null \
  || echo "  (could not retrieve image details)"

echo ""
if [[ "${FAILED}" -eq 0 ]]; then
  echo "✅ Deployment validation passed for ${ENVIRONMENT} (artifact: ${ARTIFACT_ID})"
  exit 0
else
  echo "❌ Deployment validation FAILED for ${ENVIRONMENT}"
  echo "   Review pod logs with: kubectl logs -n ${NAMESPACE} -l app=teamtasks-api --previous"
  exit 1
fi
