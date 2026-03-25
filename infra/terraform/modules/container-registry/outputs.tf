output "login_server" {
  description = "ACR login server URL."
  value       = azurerm_container_registry.main.login_server
}

output "name" {
  description = "ACR resource name."
  value       = azurerm_container_registry.main.name
}

output "id" {
  description = "ACR resource ID."
  value       = azurerm_container_registry.main.id
}
