output "app_fqdn" {
  description = "Container App FQDN"
  value       = azurerm_container_app.this.latest_revision_fqdn
}

output "app_id" {
  description = "Container App resource ID"
  value       = azurerm_container_app.this.id
}

output "environment_id" {
  description = "Container Apps environment resource ID"
  value       = azurerm_container_app_environment.this.id
}
