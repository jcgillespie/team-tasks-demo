resource "azurerm_key_vault" "this" {
  name                       = substr(replace("${var.naming_prefix}-kv", "-", ""), 0, 24)
  location                   = var.location
  resource_group_name        = var.resource_group_name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7
  purge_protection_enabled   = true
  tags                       = var.tags
}

data "azurerm_client_config" "current" {}

resource "azurerm_mssql_server" "this" {
  name                         = "${var.naming_prefix}-sql"
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"
  tags                         = var.tags
}

resource "azurerm_mssql_database" "this" {
  name      = "teamtasks"
  server_id = azurerm_mssql_server.this.id
  sku_name  = "Basic"
  tags      = var.tags
}
