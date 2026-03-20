resource "azurerm_service_plan" "plan" {
  name                = var.plan_name
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "app" {
  name                = var.app_name
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    always_on = false # F1 tier does not support always_on

    application_stack {
      dotnet_version = "9.0"
    }
  }

  app_settings = var.app_settings
}
