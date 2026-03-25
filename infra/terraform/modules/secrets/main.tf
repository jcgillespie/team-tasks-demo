data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "main" {
  name                        = "${var.name_prefix}-${var.environment}-kv"
  resource_group_name         = var.resource_group_name
  location                    = var.location
  sku_name                    = var.sku
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  enable_rbac_authorization   = true
  purge_protection_enabled    = true
  soft_delete_retention_days  = 7
  tags                        = var.tags
}

# Grant the workload managed identity read access to secrets
resource "azurerm_role_assignment" "workload_secret_reader" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.workload_identity_object_id
}

# Allow GitHub Actions (CI) to write secrets during provisioning
resource "azurerm_role_assignment" "cicd_secret_officer" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets Officer"
  principal_id         = var.cicd_identity_object_id
}
