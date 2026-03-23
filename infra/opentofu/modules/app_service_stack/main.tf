locals {
  sql_connection_reference = var.key_vault_uri == "" ? "" : "@Microsoft.KeyVault(SecretUri=${var.key_vault_uri}secrets/sql-default/)"
}

resource "azurerm_log_analytics_workspace" "this" {
  name                = "${var.naming_prefix}-log"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = var.tags
}

resource "azurerm_application_insights" "this" {
  name                = "${var.naming_prefix}-appi"
  location            = var.location
  resource_group_name = var.resource_group_name
  workspace_id        = azurerm_log_analytics_workspace.this.id
  application_type    = "web"
  tags                = var.tags
}

resource "azurerm_service_plan" "this" {
  name                = "${var.naming_prefix}-plan"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = "B1"
  tags                = var.tags
}

resource "azurerm_linux_web_app" "frontend" {
  name                = "${var.naming_prefix}-web"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.this.id
  https_only          = true
  tags                = var.tags

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    APPINSIGHTS_CONNECTION_STRING = azurerm_application_insights.this.connection_string
    WEBSITE_RUN_FROM_PACKAGE      = "1"
  }

  site_config {
    always_on = false
  }
}

resource "azurerm_linux_web_app" "api" {
  name                = "${var.naming_prefix}-api"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.this.id
  https_only          = true
  tags                = var.tags

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    APPINSIGHTS_CONNECTION_STRING = azurerm_application_insights.this.connection_string
    ConnectionStrings__DefaultConnection = local.sql_connection_reference
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = "false"
  }

  site_config {
    always_on = true
    application_stack {
      dotnet_version = "10.0"
    }
    cors {
      allowed_origins     = var.allowed_origins
      support_credentials = false
    }
  }
}

resource "azurerm_linux_web_app_slot" "frontend_staging" {
  name           = "staging"
  app_service_id = azurerm_linux_web_app.frontend.id
  https_only     = true
  tags           = var.tags

  site_config {}
}

resource "azurerm_linux_web_app_slot" "api_staging" {
  name           = "staging"
  app_service_id = azurerm_linux_web_app.api.id
  https_only     = true
  tags           = var.tags

  site_config {
    application_stack {
      dotnet_version = "10.0"
    }
  }
}
