# ---------------------------------------------------------------------------
# Shared variable schema for Team Tasks infrastructure
# Override values in each environment's terraform.tfvars
# ---------------------------------------------------------------------------

variable "subscription_id" {
  description = "Azure subscription ID to deploy resources into."
  type        = string
}

variable "location" {
  description = "Azure region for all resources (e.g. eastus2, westeurope)."
  type        = string
  default     = "eastus2"
}

variable "environment" {
  description = "Deployment environment name. Must be one of: development, stage, production."
  type        = string
  validation {
    condition     = contains(["development", "stage", "production"], var.environment)
    error_message = "environment must be one of: development, stage, production."
  }
}

variable "name_prefix" {
  description = "Short prefix used to name Azure resources (e.g. 'tt' for Team Tasks)."
  type        = string
  default     = "tt"
}

variable "tags" {
  description = "Tags applied to all provisioned resources."
  type        = map(string)
  default     = {}
}

# --- AKS ---

variable "aks_node_count" {
  description = "Initial number of nodes in the default AKS node pool."
  type        = number
  default     = 2
}

variable "aks_node_vm_size" {
  description = "VM size for AKS node pool."
  type        = string
  default     = "Standard_D2s_v3"
}

variable "aks_kubernetes_version" {
  description = "Kubernetes version for the AKS cluster."
  type        = string
  default     = null # Uses the latest supported version when null.
}

# --- Networking ---

variable "vnet_address_space" {
  description = "CIDR block for the virtual network."
  type        = list(string)
  default     = ["10.10.0.0/16"]
}

variable "aks_subnet_cidr" {
  description = "CIDR block for the AKS nodes subnet."
  type        = string
  default     = "10.10.1.0/24"
}

# --- Container Registry ---

variable "acr_sku" {
  description = "ACR SKU (Basic, Standard, Premium)."
  type        = string
  default     = "Standard"
}

# --- Key Vault ---

variable "key_vault_sku" {
  description = "Key Vault SKU (standard, premium)."
  type        = string
  default     = "standard"
}

# --- GitHub OIDC ---

variable "github_org" {
  description = "GitHub organisation or user name that owns the repository."
  type        = string
}

variable "github_repo" {
  description = "GitHub repository name (without owner)."
  type        = string
  default     = "team-tasks-demo"
}
