data "azurerm_client_config" "current" {}

module "resource_group" {
  source      = "../../modules/resource-group"
  name        = "${var.project}-${var.environment}-rg"
  location    = var.location
  environment = var.environment
  tags        = var.tags
}

module "identity" {
  source              = "../../modules/identity"
  project             = var.project
  environment         = var.environment
  resource_group_name = module.resource_group.name
  resource_group_id   = module.resource_group.id
  location            = var.location
  github_repo         = var.github_repo
  github_environments = [var.environment]
  tags                = var.tags
}

module "acr" {
  source                = "../../modules/acr"
  project               = var.project
  environment           = var.environment
  resource_group_name   = module.resource_group.name
  location              = var.location
  sku                   = "Standard"
  identity_principal_id = module.identity.principal_id
  tags                  = var.tags
}

module "keyvault" {
  source                = "../../modules/keyvault"
  project               = var.project
  environment           = var.environment
  resource_group_name   = module.resource_group.name
  location              = var.location
  tenant_id             = data.azurerm_client_config.current.tenant_id
  identity_principal_id = module.identity.principal_id
  tags                  = var.tags
}

module "sql" {
  source              = "../../modules/sql"
  project             = var.project
  environment         = var.environment
  resource_group_name = module.resource_group.name
  location            = var.location
  admin_login         = var.sql_admin_login
  admin_password      = var.sql_admin_password
  db_sku              = "S1"
  tags                = var.tags
}

module "app" {
  source               = "../../modules/app"
  project              = var.project
  environment          = var.environment
  resource_group_name  = module.resource_group.name
  location             = var.location
  identity_id          = module.identity.id
  acr_login_server     = module.acr.login_server
  image_tag            = var.image_tag
  db_connection_string = module.sql.connection_string
  tags                 = var.tags
}
