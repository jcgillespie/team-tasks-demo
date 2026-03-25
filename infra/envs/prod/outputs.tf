output "resource_group_name" {
  value = module.resource_group.name
}

output "acr_login_server" {
  value = module.acr.login_server
}

output "keyvault_uri" {
  value = module.keyvault.vault_uri
}

output "app_fqdn" {
  value = module.app.app_fqdn
}

output "identity_client_id" {
  value = module.identity.client_id
}

output "sql_server_fqdn" {
  value = module.sql.server_fqdn
}
