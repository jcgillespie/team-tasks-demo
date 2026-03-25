# Environment Setup Guide

This guide walks through provisioning a new Team Tasks environment on Azure AKS using Terraform.

## Prerequisites

- Terraform >= 1.6 installed
- Azure CLI (`az`) installed and authenticated (`az login`)
- Subscription-level permissions to create AKS, ACR, networking, identity, and Key Vault resources
- GitHub repository admin access (for OIDC federation and environment protection rules)

## Step 1: Configure the Terraform Backend

The Terraform backend uses Azure Blob Storage for remote state.  
Create the storage account and container once per subscription (not per environment):

```bash
az group create --name teamtasks-tfstate --location eastus2
az storage account create \
  --name teamtaskstfstate \
  --resource-group teamtasks-tfstate \
  --sku Standard_LRS \
  --allow-blob-public-access false
az storage container create \
  --name tfstate \
  --account-name teamtaskstfstate
```

## Step 2: Configure Environment Variables

Copy the example variables file and fill in your subscription values:

```bash
# For development:
cp infra/terraform/environments/development/terraform.tfvars.example \
   infra/terraform/environments/development/terraform.tfvars

# For stage:
cp infra/terraform/environments/stage/terraform.tfvars.example \
   infra/terraform/environments/stage/terraform.tfvars

# For production:
cp infra/terraform/environments/production/terraform.tfvars.example \
   infra/terraform/environments/production/terraform.tfvars
```

Edit each `terraform.tfvars` with the values for your subscription, location, and naming prefix.  
**Never commit `terraform.tfvars` files — they are excluded by `.gitignore`.**

## Step 3: Provision the Environment

```bash
cd infra/terraform/environments/development

# Initialise with remote backend
terraform init \
  -backend-config="resource_group_name=teamtasks-tfstate" \
  -backend-config="storage_account_name=teamtaskstfstate" \
  -backend-config="container_name=tfstate" \
  -backend-config="key=development.tfstate"

# Validate configuration
terraform validate

# Review the plan
terraform plan -out=tfplan

# Apply
terraform apply tfplan
```

Repeat for `stage` and `production`, substituting the environment name in the backend key.

## Step 4: Capture Infrastructure Outputs

After provisioning, capture the Terraform outputs needed by GitHub Actions and Kubernetes:

```bash
terraform output -json > /tmp/tf-outputs.json
cat /tmp/tf-outputs.json
```

Key outputs used by pipelines:

| Output | Used By |
|--------|---------|
| `aks_cluster_name` | `kubectl` context + GitHub Actions deploy step |
| `aks_resource_group` | GitHub Actions deploy step |
| `acr_login_server` | Docker image tagging and publishing |
| `key_vault_uri` | Application configuration |
| `managed_identity_client_id` | AKS pod identity annotation |

Store these as GitHub Actions environment variables or repository variables (not secrets, unless sensitive).

## Step 5: Configure GitHub OIDC Federation

See [`infra/OIDC_SETUP.md`](OIDC_SETUP.md) for the full OIDC federation setup.

## Step 6: Configure GitHub Environments

In the GitHub repository settings:

1. Create environments named `development`, `stage`, and `production`.
2. For `production`, add required reviewers and enable **Required reviewers** protection.
3. Add environment-scoped variables for each environment (`AKS_CLUSTER_NAME`, `ACR_LOGIN_SERVER`, etc.).

## Step 7: Verify the Deployment

```bash
# Confirm AKS nodes are ready
kubectl get nodes

# Confirm namespace exists
kubectl get namespace development

# Check deployed workloads (after first pipeline run)
kubectl get pods -n development
kubectl get ingress -n development
```

## Reprovisioning an Existing Environment

Terraform is idempotent. To re-run provisioning:

```bash
terraform plan -out=tfplan
terraform apply tfplan
```

No unintended resources will be created if inputs are unchanged.

## Tearing Down an Environment

```bash
terraform destroy
```

> **Warning**: This permanently deletes all Azure resources in the environment.  
> Do not run against production without explicit authorisation.
