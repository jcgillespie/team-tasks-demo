locals {
  tags = merge(var.tags, {
    environment = var.environment
    project     = "teamtasks"
    managed-by  = "opentofu"
  })

  registry_name = substr(lower("${replace(var.project, "/[^0-9A-Za-z]/", "")}${replace(var.environment, "/[^0-9A-Za-z]/", "")}acr"), 0, 50)
}

resource "azurerm_container_registry" "this" {
  name                = local.registry_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = false
  tags                = local.tags
}

resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.this.id
  role_definition_name = "AcrPull"
  principal_id         = var.identity_principal_id
}
