#!/usr/bin/env bash
set -euo pipefail

# Run this ONCE before first `tofu init`.
# This backend is intentionally not managed by OpenTofu to avoid circular dependencies.
# Run: bash infra/bootstrap/create-state-backend.sh

RESOURCE_GROUP="${RESOURCE_GROUP:-teamtasks-tfstate-rg}"
STORAGE_ACCOUNT="${STORAGE_ACCOUNT:-teamtaskstfstate}"
CONTAINER_NAME="${CONTAINER_NAME:-tfstate}"
LOCATION="${LOCATION:-eastus}"

echo "Creating resource group: ${RESOURCE_GROUP} (${LOCATION})"
az group create \
  --name "${RESOURCE_GROUP}" \
  --location "${LOCATION}" \
  --output none

echo "Creating storage account: ${STORAGE_ACCOUNT}"
az storage account create \
  --name "${STORAGE_ACCOUNT}" \
  --resource-group "${RESOURCE_GROUP}" \
  --location "${LOCATION}" \
  --sku Standard_LRS \
  --encryption-services blob \
  --allow-blob-public-access false \
  --https-only true \
  --min-tls-version TLS1_2 \
  --output none

echo "Creating state container: ${CONTAINER_NAME}"
az storage container create \
  --name "${CONTAINER_NAME}" \
  --account-name "${STORAGE_ACCOUNT}" \
  --auth-mode login \
  --output none || true

echo "OpenTofu backend ready"
echo "storage_account_name=${STORAGE_ACCOUNT}"
echo "container_name=${CONTAINER_NAME}"
