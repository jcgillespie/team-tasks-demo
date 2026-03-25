# GitHub Actions OIDC Federation Setup

This guide configures GitHub Actions to authenticate to Azure using OpenID Connect (OIDC) federation. This removes the need for long-lived service principal secrets and follows the principle of least privilege.

## Overview

GitHub Actions can request a short-lived OIDC token from GitHub's token service. Azure is configured to trust this token via a **Federated Identity Credential** on a User-Assigned Managed Identity. The workflow exchanges the GitHub OIDC token for an Azure access token, scoped to the identity's role assignments.

## Step 1: The Terraform Setup (Automated)

The `identity` Terraform module creates:
1. A **User-Assigned Managed Identity** per environment
2. A **Federated Identity Credential** binding the managed identity to the GitHub environment
3. A **role assignment** granting the identity `Contributor` on the environment resource group

After `terraform apply`, the outputs `managed_identity_client_id` and `managed_identity_object_id` are available.

## Step 2: Configure GitHub Repository Secrets

For each environment (`development`, `stage`, `production`), add the following **GitHub repository secrets** (or environment-scoped secrets):

| Secret | Value | Where to Find |
|--------|-------|--------------|
| `AZURE_CLIENT_ID` | Client ID of the managed identity | `terraform output managed_identity_client_id` |
| `AZURE_TENANT_ID` | Azure Active Directory tenant ID | Azure Portal → AAD → Overview |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID | Azure Portal → Subscriptions |

> Note: Use environment-scoped secrets when different environments use different identities.

## Step 3: Configure GitHub Actions Workflows

Workflows that interact with Azure must include:

```yaml
permissions:
  id-token: write  # Required for OIDC token request
  contents: read

steps:
  - name: Azure login (OIDC)
    uses: azure/login@v2
    with:
      client-id: ${{ secrets.AZURE_CLIENT_ID }}
      tenant-id: ${{ secrets.AZURE_TENANT_ID }}
      subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
```

All workflows in this repository already include this pattern.

## Step 4: Verify OIDC Authentication

Trigger any workflow that uses `azure/login@v2`. In the workflow run logs, look for:
```
Login successful.
```

If authentication fails, check:
1. The federated credential subject matches the workflow's `ref` and environment exactly
2. The managed identity has the required role assignments
3. The GitHub environment name matches the `environment:` key in the federated credential

## Federated Credential Subject Format

The subject claim must exactly match what GitHub sends. The Terraform module configures:
```
repo:<github_org>/<github_repo>:environment:<environment_name>
```

For environment-less workflows (e.g., PR validation), use:
```
repo:<github_org>/<github_repo>:ref:refs/heads/main
```

## Security Notes

- OIDC tokens are short-lived and scoped to the workflow run
- No credentials are stored in the repository
- Each environment uses a separate identity (separate blast radius)
- Role assignments are scoped to the environment resource group (not subscription)
