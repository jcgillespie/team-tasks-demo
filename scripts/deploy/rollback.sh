#!/usr/bin/env bash
# rollback.sh
#
# Rolls back a Team Tasks environment to a previously deployed artifact.
# Usage:
#   rollback.sh <environment> <artifact-id>
#
# The artifact-id is a git SHA that was previously pushed as a container image.
# Both the API and client images with that tag must exist in ACR.

set -euo pipefail

ENVIRONMENT="${1:-}"
ARTIFACT_ID="${2:-}"

if [[ -z "${ENVIRONMENT}" || -z "${ARTIFACT_ID}" ]]; then
  echo "Usage: $0 <environment> <artifact-id>" >&2
  echo "  environment: development | stage | production" >&2
  echo "  artifact-id: git SHA of the artifact to roll back to" >&2
  exit 1
fi

NAMESPACE="${ENVIRONMENT}"

echo "⏪ Rolling back ${NAMESPACE} to artifact: ${ARTIFACT_ID}"
echo ""

# Resolve ACR login server from kubectl context or environment variable
ACR_LOGIN_SERVER="${ACR_LOGIN_SERVER:-}"

if [[ -z "${ACR_LOGIN_SERVER}" ]]; then
  echo "❌ ACR_LOGIN_SERVER environment variable is required." >&2
  echo "   Set it to your ACR login server, e.g.: export ACR_LOGIN_SERVER=ttacr.azurecr.io" >&2
  exit 1
fi

API_IMAGE="${ACR_LOGIN_SERVER}/teamtasks/api:${ARTIFACT_ID}"
CLIENT_IMAGE="${ACR_LOGIN_SERVER}/teamtasks/client:${ARTIFACT_ID}"

echo "API image:    ${API_IMAGE}"
echo "Client image: ${CLIENT_IMAGE}"
echo ""

# Confirm rollback
read -r -p "Proceed with rollback in ${NAMESPACE}? [y/N] " confirm
if [[ ! "${confirm}" =~ ^[Yy]$ ]]; then
  echo "Rollback cancelled."
  exit 0
fi

echo ""
echo "Setting API image..."
kubectl set image deployment/teamtasks-api \
  teamtasks-api="${API_IMAGE}" \
  -n "${NAMESPACE}"

echo "Setting client image..."
kubectl set image deployment/teamtasks-client \
  teamtasks-client="${CLIENT_IMAGE}" \
  -n "${NAMESPACE}"

echo ""
echo "Waiting for rollouts to complete..."
kubectl rollout status deployment/teamtasks-api -n "${NAMESPACE}" --timeout=5m
kubectl rollout status deployment/teamtasks-client -n "${NAMESPACE}" --timeout=5m

echo ""
echo "✅ Rollback complete."
echo ""
echo "Verify the rollback:"
echo "  kubectl get pods -n ${NAMESPACE}"
echo "  kubectl logs -n ${NAMESPACE} -l app=teamtasks-api --tail=50"
