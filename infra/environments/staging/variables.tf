variable "resource_group_name" {
  type        = string
  description = "Name of the Azure resource group for staging"
  default     = "rg-claims-staging"
}

variable "location" {
  type        = string
  description = "Azure region for all resources"
  default     = "uksouth"
}

variable "sql_admin_username" {
  type        = string
  description = "SQL Server administrator login"
  default     = "sqladmin"
}

variable "sql_admin_password" {
  type        = string
  description = "SQL Server administrator password — passed via CI secret, never committed"
  sensitive   = true
}

variable "existing_plan_name" {
  type        = string
  description = "Name of the existing F1 App Service Plan to reuse"
}

variable "existing_plan_resource_group" {
  type        = string
  description = "Resource group of the existing F1 App Service Plan"
}
