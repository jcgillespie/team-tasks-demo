variable "name_prefix" {
  description = "Short prefix for resource naming."
  type        = string
}

variable "environment" {
  description = "Deployment environment (development, stage, production)."
  type        = string
}

variable "resource_group_name" {
  description = "Resource group to deploy ACR into."
  type        = string
}

variable "location" {
  description = "Azure region."
  type        = string
}

variable "sku" {
  description = "ACR SKU (Basic, Standard, Premium)."
  type        = string
  default     = "Standard"
}

variable "aks_kubelet_object_id" {
  description = "Object ID of the AKS kubelet managed identity for AcrPull role assignment."
  type        = string
}

variable "tags" {
  description = "Resource tags."
  type        = map(string)
  default     = {}
}
