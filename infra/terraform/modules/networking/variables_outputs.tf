variable "name_prefix" { type = string }
variable "environment" { type = string }
variable "resource_group_name" { type = string }
variable "location" { type = string }
variable "vnet_address_space" { type = list(string) }
variable "aks_subnet_cidr" { type = string }
variable "tags" { type = map(string); default = {} }

output "vnet_id" { value = azurerm_virtual_network.main.id }
output "aks_subnet_id" { value = azurerm_subnet.aks.id }
