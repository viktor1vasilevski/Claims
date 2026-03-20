variable "resource_group_name" {
  type = string
}

variable "app_name" {
  type = string
}

variable "existing_plan_name" {
  type        = string
  description = "Name of the existing App Service Plan to use"
}

variable "existing_plan_resource_group" {
  type        = string
  description = "Resource group of the existing App Service Plan"
}

variable "app_settings" {
  type    = map(string)
  default = {}
}
