data "azurerm_client_config" "current" {}

# User-assigned managed identity for AKS workloads
resource "azurerm_user_assigned_identity" "workload" {
  name                = "${var.name_prefix}-${var.environment}-workload-id"
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

# Federated identity credential — allows GitHub Actions to assume this identity
resource "azurerm_federated_identity_credential" "github_actions" {
  name                = "github-actions-${var.environment}"
  resource_group_name = var.resource_group_name
  parent_id           = azurerm_user_assigned_identity.workload.id
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  subject             = "repo:${var.github_org}/${var.github_repo}:environment:${var.environment}"
}

# Contributor on the resource group (scoped for AKS + ACR operations)
resource "azurerm_role_assignment" "contributor" {
  scope                = "/subscriptions/${var.subscription_id}/resourceGroups/${var.resource_group_name}"
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.workload.principal_id
}
