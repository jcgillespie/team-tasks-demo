variable "name_prefix" { type = string }
variable "environment" { type = string }
variable "resource_group_name" { type = string }
variable "location" { type = string }
variable "subscription_id" { type = string }
variable "github_org" { type = string }
variable "github_repo" { type = string }
variable "tags" { type = map(string); default = {} }

output "workload_identity_client_id" { value = azurerm_user_assigned_identity.workload.client_id }
output "workload_identity_object_id" { value = azurerm_user_assigned_identity.workload.principal_id }
output "workload_identity_id" { value = azurerm_user_assigned_identity.workload.id }
