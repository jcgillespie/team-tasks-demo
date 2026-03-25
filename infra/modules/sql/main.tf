locals {
  tags = merge(var.tags, {
    environment = var.environment
    project     = "teamtasks"
    managed-by  = "opentofu"
  })

  server_name = substr(lower("${var.project}-${var.environment}-sql"), 0, 63)
}

resource "azurerm_mssql_server" "this" {
  name                          = local.server_name
  resource_group_name           = var.resource_group_name
  location                      = var.location
  version                       = "12.0"
  administrator_login           = var.admin_login
  administrator_login_password  = var.admin_password
  minimum_tls_version           = "1.2"
  public_network_access_enabled = true
  tags                          = local.tags
}

resource "azurerm_mssql_database" "this" {
  name           = "teamtasks"
  server_id      = azurerm_mssql_server.this.id
  sku_name       = var.db_sku
  max_size_gb    = 2
  zone_redundant = false
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.this.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
