data "azurerm_service_plan" "existing" {
  name                = var.existing_plan_name
  resource_group_name = var.existing_plan_resource_group
}

resource "azurerm_windows_web_app" "app" {
  name                = var.app_name
  resource_group_name = var.resource_group_name
  location            = data.azurerm_service_plan.existing.location
  service_plan_id     = data.azurerm_service_plan.existing.id

  site_config {
    always_on = false # F1 tier does not support always_on

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v9.0"
    }
  }

  app_settings = var.app_settings
}
