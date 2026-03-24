variable "application_name" { type = string }
variable "environment" { type = string }
variable "resource_group_name" { type = string }
variable "location" { type = string }
variable "owner" { type = string }
variable "cost_center" { type = string }
variable "allowed_origins" { type = list(string) }
variable "sql_admin_username" { type = string }
variable "sql_admin_password" {
  type      = string
  sensitive = true
}
