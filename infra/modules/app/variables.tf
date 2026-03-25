variable "project" {
  description = "Project short name"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "identity_id" {
  description = "User-assigned managed identity ID"
  type        = string
}

variable "acr_login_server" {
  description = "ACR login server"
  type        = string
}

variable "image_name" {
  description = "Container image name"
  type        = string
  default     = "teamtasks-api"
}

variable "image_tag" {
  description = "Container image tag"
  type        = string
  default     = "latest"
}

variable "db_connection_string" {
  description = "Database connection string"
  type        = string
  sensitive   = true
}

variable "aspnet_environment" {
  description = "ASPNETCORE_ENVIRONMENT value"
  type        = string
  default     = "Production"
}

variable "tags" {
  description = "Additional tags"
  type        = map(string)
  default     = {}
}
