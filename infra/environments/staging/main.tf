terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    time = {
      source  = "hashicorp/time"
      version = "~> 0.12"
    }
  }
}

# Authentication via environment variables in CI:
#   ARM_CLIENT_ID, ARM_CLIENT_SECRET, ARM_TENANT_ID, ARM_SUBSCRIPTION_ID
provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

module "servicebus" {
  source              = "../../modules/servicebus"
  namespace_name      = "sb-claims-staging"
  queue_name          = "audit-queue"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
}

module "cosmos" {
  source              = "../../modules/cosmos"
  account_name        = "cosmos-claims-staging"
  database_name       = "ClaimsStaging"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
}

# Workaround: azurerm provider ~4.0 reads vulnerability assessment settings
# immediately after SQL server creation, before Azure fully propagates the resource.
resource "time_sleep" "wait_for_resource_group" {
  depends_on      = [azurerm_resource_group.rg]
  create_duration = "30s"
}

module "sql" {
  source              = "../../modules/sql"
  server_name         = "sql-claims-staging"
  database_name       = "ClaimsDb"
  admin_username      = var.sql_admin_username
  admin_password      = var.sql_admin_password
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  depends_on = [time_sleep.wait_for_resource_group]
}

module "appservice" {
  source                       = "../../modules/appservice"
  app_name                     = "app-claims-staging"
  resource_group_name          = azurerm_resource_group.rg.name
  existing_plan_name           = var.existing_plan_name
  existing_plan_resource_group = var.existing_plan_resource_group

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                  = "Staging"
    "ConnectionStrings__SqlServer"            = module.sql.connection_string
    "ConnectionStrings__MongoDb"              = module.cosmos.connection_string
    "MongoDb__DatabaseName"                   = "ClaimsStaging"
    "Messaging__Provider"                     = "ServiceBus"
    "Messaging__ServiceBus__ConnectionString" = module.servicebus.connection_string
    "Messaging__ServiceBus__QueueName"        = module.servicebus.queue_name
  }
}

output "webapp_name" {
  value = module.appservice.webapp_name
}

output "webapp_url" {
  value = "https://${module.appservice.default_hostname}"
}
