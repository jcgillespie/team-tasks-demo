output "frontend_name" {
  value = azurerm_linux_web_app.frontend.name
}

output "frontend_default_hostname" {
  value = azurerm_linux_web_app.frontend.default_hostname
}

output "frontend_staging_slot_name" {
  value = azurerm_linux_web_app_slot.frontend_staging.name
}

output "api_name" {
  value = azurerm_linux_web_app.api.name
}

output "api_default_hostname" {
  value = azurerm_linux_web_app.api.default_hostname
}

output "api_staging_slot_name" {
  value = azurerm_linux_web_app_slot.api_staging.name
}

output "production_slot_template" {
  value = {
    frontend = azurerm_linux_web_app_slot.frontend_staging.name
    api      = azurerm_linux_web_app_slot.api_staging.name
  }
}

output "app_settings_template" {
  value = {
    app_insights_connection_string = azurerm_application_insights.this.connection_string
    sql_connection_reference       = local.sql_connection_reference
  }
  sensitive = true
}

output "application_insights_connection_string" {
  value     = azurerm_application_insights.this.connection_string
  sensitive = true
}
