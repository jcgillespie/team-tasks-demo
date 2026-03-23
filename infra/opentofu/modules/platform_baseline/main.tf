locals {
  app = replace(lower(var.application_name), "_", "-")
  env = lower(var.environment)

  naming_prefix = "${local.app}-${local.env}"

  tags = {
    application = local.app
    environment = local.env
    owner       = var.owner
    cost_center = var.cost_center
    managed_by  = "opentofu"
  }
}
