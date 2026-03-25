# Secret Rotation and Management

This document describes how deployment secrets are managed and rotated for Team Tasks environments.

## Secret Inventory

| Secret | Storage | Consumer | Rotation Owner |
|--------|---------|----------|---------------|
| Database connection strings | Azure Key Vault | API pod (via env var) | Platform team |
| API keys / third-party integrations | Azure Key Vault | API pod | Application team |
| `AZURE_CLIENT_ID` | GitHub Actions secret | CI/CD workflows | Platform team |
| `AZURE_TENANT_ID` | GitHub Actions secret | CI/CD workflows | Platform team |
| `AZURE_SUBSCRIPTION_ID` | GitHub Actions secret | CI/CD workflows | Platform team |
| TF backend storage account name | GitHub Actions secret | Terraform workflows | Platform team |

## What Is NOT a Secret

| Item | Where Stored | Why Safe |
|------|-------------|----------|
| ACR login server URL | GitHub Actions variable | Not sensitive; public DNS |
| AKS cluster name | GitHub Actions variable | Not sensitive |
| Resource group name | GitHub Actions variable | Not sensitive |
| Key Vault URI | Terraform output | Not sensitive; no credentials |

## Azure Key Vault

Application runtime secrets are stored in Key Vault and accessed by pods via the Managed Identity.
Secrets are referenced by name; values are never in source code or environment variables committed to Git.

### Adding a New Secret

```bash
az keyvault secret set \
  --vault-name <vault-name> \
  --name <secret-name> \
  --value "<secret-value>"
```

### Accessing a Secret in Code

Configure the application to read from Key Vault using `Azure.Extensions.AspNetCore.Configuration.Secrets`:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVaultUri"]!),
    new DefaultAzureCredential());
```

The `DefaultAzureCredential` uses the pod's Managed Identity in AKS automatically.

### Rotating a Secret

1. Generate the new secret value
2. Update the secret in Key Vault:
   ```bash
   az keyvault secret set --vault-name <vault-name> --name <secret-name> --value "<new-value>"
   ```
3. Restart affected pods to pick up the new value:
   ```bash
   kubectl rollout restart deployment/teamtasks-api -n <environment>
   ```

## GitHub Actions Secrets

GitHub Actions secrets (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`) authenticate via OIDC and do not contain sensitive values (they are IDs, not credentials). They rarely need rotation.

If the managed identity is deleted and recreated (e.g., `terraform destroy` + `terraform apply`):
1. Update `AZURE_CLIENT_ID` with the new `managed_identity_client_id` from Terraform output
2. The `AZURE_TENANT_ID` and `AZURE_SUBSCRIPTION_ID` values are stable

## Security Rules

- **Never commit secret values** to source code or configuration files
- **Never log secret values** in CI/CD output
- **Review and rotate** secrets annually, or immediately after a suspected compromise
- **Use Key Vault versioning** — old versions are retained for rollback but can be disabled/deleted
- **Audit access**: `az monitor activity-log list --resource-group <rg> --query "[?authorization.action=='Microsoft.KeyVault/vaults/secrets/read']"`
