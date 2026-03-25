# GitHub Actions OIDC federated identity credentials.
# Allows GitHub Actions to authenticate to Azure without client secrets.
resource "azurerm_federated_identity_credential" "github" {
  for_each = toset(var.github_environments)

  name                = "github-${each.value}"
  resource_group_name = var.resource_group_name
  parent_id           = azurerm_user_assigned_identity.this.id
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  subject             = "repo:${var.github_repo}:environment:${each.value}"
}
