variable "naming_prefix" {
  type = string
}

variable "location" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "tags" {
  type = map(string)
}

variable "allowed_origins" {
  type = list(string)
}

variable "key_vault_uri" {
  type    = string
  default = ""
}
