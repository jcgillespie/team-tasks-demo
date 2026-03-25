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

variable "resource_group_id" {
  description = "Resource group ID for Contributor role assignment"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "github_repo" {
  description = "GitHub repository in owner/repo format for OIDC federation"
  type        = string
}

variable "github_environments" {
  description = "GitHub Actions environment names to create OIDC credentials for"
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Additional tags"
  type        = map(string)
  default     = {}
}
