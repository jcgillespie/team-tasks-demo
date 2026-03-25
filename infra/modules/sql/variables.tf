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

variable "admin_login" {
  description = "SQL admin login"
  type        = string
  sensitive   = true
}

variable "admin_password" {
  description = "SQL admin password"
  type        = string
  sensitive   = true
}

variable "db_sku" {
  description = "SQL database SKU"
  type        = string
  default     = "Basic"
}

variable "tags" {
  description = "Additional tags"
  type        = map(string)
  default     = {}
}
