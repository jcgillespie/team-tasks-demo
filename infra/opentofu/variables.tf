variable "application_name" {
  description = "Application name prefix used for resource naming."
  type        = string
}

variable "environment" {
  description = "Deployment environment name."
  type        = string
}

variable "location" {
  description = "Azure location for resources."
  type        = string
}

variable "owner" {
  description = "Owning team identifier tag value."
  type        = string
}

variable "cost_center" {
  description = "Cost center tag value."
  type        = string
}
