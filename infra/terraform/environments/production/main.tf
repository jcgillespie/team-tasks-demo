terraform {
  required_version = ">= 1.6"
  backend "azurerm" {}
}

locals {
  common_tags = merge(var.tags, {
    environment = var.environment
    project     = "team-tasks"
    managed_by  = "terraform"
  })
}

module "networking" {
  source              = "../../modules/networking"
  name_prefix         = var.name_prefix
  environment         = var.environment
  resource_group_name = "${var.name_prefix}-${var.environment}-rg"
  location            = var.location
  vnet_address_space  = var.vnet_address_space
  aks_subnet_cidr     = var.aks_subnet_cidr
  tags                = local.common_tags
}

module "aks" {
  source             = "../../modules/aks"
  name_prefix        = var.name_prefix
  environment        = var.environment
  location           = var.location
  node_count         = var.aks_node_count
  node_vm_size       = var.aks_node_vm_size
  kubernetes_version = var.aks_kubernetes_version
  aks_subnet_id      = module.networking.aks_subnet_id
  tags               = local.common_tags
}

module "identity" {
  source              = "../../modules/identity"
  name_prefix         = var.name_prefix
  environment         = var.environment
  resource_group_name = module.aks.resource_group_name
  location            = var.location
  subscription_id     = var.subscription_id
  github_org          = var.github_org
  github_repo         = var.github_repo
  tags                = local.common_tags
}

module "container_registry" {
  source                = "../../modules/container-registry"
  name_prefix           = var.name_prefix
  environment           = var.environment
  resource_group_name   = module.aks.resource_group_name
  location              = var.location
  sku                   = var.acr_sku
  aks_kubelet_object_id = module.aks.kubelet_object_id
  tags                  = local.common_tags
}

module "secrets" {
  source                      = "../../modules/secrets"
  name_prefix                 = var.name_prefix
  environment                 = var.environment
  resource_group_name         = module.aks.resource_group_name
  location                    = var.location
  sku                         = var.key_vault_sku
  workload_identity_object_id = module.identity.workload_identity_object_id
  cicd_identity_object_id     = module.identity.workload_identity_object_id
  tags                        = local.common_tags
}
