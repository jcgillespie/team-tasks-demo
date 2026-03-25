variable "environment" {
  type    = string
  default = "prod"
}

variable "location" {
  type    = string
  default = "eastus"
}

variable "project" {
  type    = string
  default = "teamtasks"
}

variable "github_repo" {
  type    = string
  default = "jcgillespie/team-tasks-demo"
}

variable "sql_admin_login" {
  type      = string
  sensitive = true
}

variable "sql_admin_password" {
  type      = string
  sensitive = true
}

variable "image_tag" {
  type    = string
  default = "latest"
}

variable "tags" {
  type    = map(string)
  default = {}
}
