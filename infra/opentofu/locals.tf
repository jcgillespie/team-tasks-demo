locals {
  normalized_app = replace(lower(var.application_name), "_", "-")
  normalized_env = lower(var.environment)

  tags = {
    application = local.normalized_app
    environment = local.normalized_env
    owner       = var.owner
    cost_center = var.cost_center
    managed_by  = "opentofu"
    repository  = "team-tasks-demo"
  }
}
