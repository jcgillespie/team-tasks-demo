variable "name_prefix" { type = string }
variable "environment" { type = string }
variable "resource_group_name" { type = string }
variable "location" { type = string }
variable "sku" { type = string; default = "standard" }
variable "workload_identity_object_id" { type = string }
variable "cicd_identity_object_id" { type = string }
variable "tags" { type = map(string); default = {} }

output "vault_uri" { value = azurerm_key_vault.main.vault_uri }
output "vault_name" { value = azurerm_key_vault.main.name }
output "vault_id" { value = azurerm_key_vault.main.id }
