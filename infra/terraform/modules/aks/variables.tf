variable "name_prefix" {
  description = "Short prefix for resource naming."
  type        = string
}

variable "environment" {
  description = "Deployment environment (development, stage, production)."
  type        = string
}

variable "location" {
  description = "Azure region."
  type        = string
}

variable "node_count" {
  description = "Number of nodes in the default node pool."
  type        = number
  default     = 2
}

variable "node_vm_size" {
  description = "VM size for default node pool."
  type        = string
  default     = "Standard_D2s_v3"
}

variable "kubernetes_version" {
  description = "Kubernetes version. Null uses latest AKS-supported version."
  type        = string
  default     = null
}

variable "aks_subnet_id" {
  description = "Subnet resource ID for AKS nodes."
  type        = string
}

variable "tags" {
  description = "Resource tags."
  type        = map(string)
  default     = {}
}
