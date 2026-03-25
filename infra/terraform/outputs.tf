# ---------------------------------------------------------------------------
# Infrastructure outputs — consumed by GitHub Actions and Kubernetes tooling
# ---------------------------------------------------------------------------

output "resource_group_name" {
  description = "Name of the Azure resource group for this environment."
  value       = module.aks.resource_group_name
}

output "aks_cluster_name" {
  description = "AKS cluster name — used by kubectl context and GitHub Actions deploy steps."
  value       = module.aks.cluster_name
}

output "aks_cluster_id" {
  description = "Full resource ID of the AKS cluster."
  value       = module.aks.cluster_id
}

output "acr_login_server" {
  description = "Container registry login server — used for image tagging and push/pull."
  value       = module.container_registry.login_server
}

output "acr_name" {
  description = "Azure Container Registry name."
  value       = module.container_registry.name
}

output "key_vault_uri" {
  description = "Key Vault URI — used for application secret references."
  value       = module.secrets.vault_uri
}

output "key_vault_name" {
  description = "Key Vault name."
  value       = module.secrets.vault_name
}

output "managed_identity_client_id" {
  description = "Client ID of the User-Assigned Managed Identity used by AKS workloads."
  value       = module.identity.workload_identity_client_id
}

output "managed_identity_object_id" {
  description = "Object ID of the User-Assigned Managed Identity."
  value       = module.identity.workload_identity_object_id
}

output "vnet_id" {
  description = "Virtual network resource ID."
  value       = module.networking.vnet_id
}
