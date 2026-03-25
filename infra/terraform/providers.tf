terraform {
  required_version = ">= 1.6"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.100"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.50"
    }
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy = false
    }
    resource_group {
      prevent_deletion_if_contains_resources = true
    }
  }

  # Authentication uses OIDC federation from GitHub Actions.
  # For local execution, ensure `az login` is current.
  # Set ARM_SUBSCRIPTION_ID via environment variable or in terraform.tfvars.
  subscription_id = var.subscription_id
}

provider "azuread" {}
