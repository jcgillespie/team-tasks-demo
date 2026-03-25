locals {
  tags = merge(var.tags, {
    environment = var.environment
    project     = "teamtasks"
    managed-by  = "opentofu"
  })

  vault_name = substr(lower("${var.project}-${var.environment}-kv"), 0, 24)
}

resource "azurerm_key_vault" "this" {
  name                          = local.vault_name
  location                      = var.location
  resource_group_name           = var.resource_group_name
  tenant_id                     = var.tenant_id
  sku_name                      = "standard"
  rbac_authorization_enabled    = true
  soft_delete_retention_days    = 7
  purge_protection_enabled      = true
  public_network_access_enabled = true
  tags                          = local.tags
}

resource "azurerm_role_assignment" "secrets_user" {
  scope                = azurerm_key_vault.this.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = var.identity_principal_id
}
