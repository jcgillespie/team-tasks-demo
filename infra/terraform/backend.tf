terraform {
  backend "azurerm" {
    # Backend values are supplied at init time via -backend-config flags or a backend config file.
    # Example:
    #   terraform init \
    #     -backend-config="resource_group_name=teamtasks-tfstate" \
    #     -backend-config="storage_account_name=teamtaskstfstate" \
    #     -backend-config="container_name=tfstate" \
    #     -backend-config="key=<environment>.tfstate"
    #
    # See infra/ENVIRONMENT_SETUP.md for full bootstrap instructions.
  }
}
