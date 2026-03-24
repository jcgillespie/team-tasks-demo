terraform {
  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

module "platform_baseline" {
  source           = "../../modules/platform_baseline"
  application_name = var.application_name
  environment      = var.environment
  location         = var.location
  owner            = var.owner
  cost_center      = var.cost_center
}

resource "azurerm_resource_group" "this" {
  name     = var.resource_group_name
  location = var.location
  tags     = module.platform_baseline.tags
}

module "app_service_stack" {
  source              = "../../modules/app_service_stack"
  naming_prefix       = module.platform_baseline.naming_prefix
  location            = var.location
  resource_group_name = azurerm_resource_group.this.name
  tags                = module.platform_baseline.tags
  allowed_origins     = var.allowed_origins
  key_vault_uri       = module.data_protection.key_vault_uri
}

module "data_protection" {
  source              = "../../modules/data_protection"
  naming_prefix       = module.platform_baseline.naming_prefix
  location            = var.location
  resource_group_name = azurerm_resource_group.this.name
  tags                = module.platform_baseline.tags
  sql_admin_username  = var.sql_admin_username
  sql_admin_password  = var.sql_admin_password
}

resource "azurerm_role_assignment" "api_kv_secrets_user" {
  scope                = module.data_protection.key_vault_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = module.app_service_stack.api_principal_id
}
