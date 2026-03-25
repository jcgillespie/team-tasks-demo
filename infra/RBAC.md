# RBAC — Role Assignments and Least-Privilege Guide

This document describes the Azure RBAC role assignments used for Team Tasks and the rationale for each.

## Principle

All identities (managed identities, GitHub Actions OIDC) are granted the **minimum permissions required** for their function.  
No long-lived secrets or service principal credentials are committed to source control.

## Identity Overview

| Identity | Purpose | Scope |
|----------|---------|-------|
| AKS System-Assigned Identity | AKS control plane operations | AKS resource group |
| AKS Kubelet Identity | Pull images from ACR | ACR resource |
| Workload Managed Identity | GitHub Actions CI/CD operations | Environment resource group |
| Workload Managed Identity (workload) | Key Vault secret access from AKS pods | Key Vault resource |

## Role Assignments

### AKS Kubelet → ACR

| Role | Scope | Reason |
|------|-------|--------|
| `AcrPull` | ACR resource | AKS pulls container images at deploy time |

Configured in: `infra/terraform/modules/container-registry/main.tf`

### GitHub Actions (Workload Identity) → Resource Group

| Role | Scope | Reason |
|------|-------|--------|
| `Contributor` | Environment resource group | Required for AKS deploy, ACR push triggers, and Terraform apply |

Configured in: `infra/terraform/modules/identity/main.tf`

### Workload Identity → Key Vault (runtime)

| Role | Scope | Reason |
|------|-------|--------|
| `Key Vault Secrets User` | Key Vault resource | Application pods read secrets at runtime |
| `Key Vault Secrets Officer` | Key Vault resource | CI/CD writes secrets during provisioning |

Configured in: `infra/terraform/modules/secrets/main.tf`

## Granting Access to a New Team Member

To give a developer read-only access to an environment's AKS cluster:

```bash
# Get AKS resource ID
AKS_ID=$(az aks show --name <cluster> --resource-group <rg> --query id -o tsv)

# Assign Azure Kubernetes Service Cluster User Role
az role assignment create \
  --assignee <user-object-id-or-upn> \
  --role "Azure Kubernetes Service Cluster User Role" \
  --scope "${AKS_ID}"
```

## Revoking Access

Revoke role assignments via the Azure Portal (IAM blade) or Azure CLI:

```bash
az role assignment delete \
  --assignee <principal-id> \
  --role "<role-name>" \
  --scope <scope>
```

## Auditing Role Assignments

```bash
# List all role assignments in a resource group
az role assignment list --resource-group <rg> --output table
```

## Security Notes

- Never grant `Owner` or `User Access Administrator` to CI/CD identities
- Review role assignments quarterly and remove stale ones
- All role assignments are managed by Terraform — manual assignments should be imported or removed
