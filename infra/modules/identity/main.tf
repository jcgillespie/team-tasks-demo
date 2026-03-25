locals {
  tags = merge(var.tags, {
    environment = var.environment
    project     = "teamtasks"
    managed-by  = "opentofu"
  })
}

resource "azurerm_user_assigned_identity" "this" {
  name                = "${var.project}-${var.environment}-identity"
  location            = var.location
  resource_group_name = var.resource_group_name
  tags                = local.tags
}

resource "azurerm_role_assignment" "contributor" {
  scope                = var.resource_group_id
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.this.principal_id
}
