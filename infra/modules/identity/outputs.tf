output "id" {
  description = "Managed identity resource ID"
  value       = azurerm_user_assigned_identity.this.id
}

output "principal_id" {
  description = "Managed identity principal ID"
  value       = azurerm_user_assigned_identity.this.principal_id
}

output "client_id" {
  description = "Managed identity client ID"
  value       = azurerm_user_assigned_identity.this.client_id
}

output "name" {
  description = "Managed identity name"
  value       = azurerm_user_assigned_identity.this.name
}

output "federated_credential_ids" {
  description = "Map of GitHub environment name to federated credential resource ID"
  value       = { for k, v in azurerm_federated_identity_credential.github : k => v.id }
}
