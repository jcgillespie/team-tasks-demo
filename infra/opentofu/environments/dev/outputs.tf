output "resource_group_name" {
  value = azurerm_resource_group.this.name
}

output "frontend_hostname" {
  value = module.app_service_stack.frontend_default_hostname
}

output "api_hostname" {
  value = module.app_service_stack.api_default_hostname
}

output "key_vault_uri" {
  value = module.data_protection.key_vault_uri
}

output "sql_server_fqdn" {
  value = module.data_protection.sql_server_fqdn
}

output "sql_database_name" {
  value = module.data_protection.sql_database_name
}
