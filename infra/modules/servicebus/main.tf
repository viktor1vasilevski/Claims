resource "azurerm_servicebus_namespace" "namespace" {
  name                = var.namespace_name
  resource_group_name = var.resource_group_name
  location            = var.location

  # Basic tier: supports queues, ~$0.05/million operations.
  # Note: Basic does NOT support topics/subscriptions — queues only.
  sku = "Basic"
}

resource "azurerm_servicebus_queue" "audit_queue" {
  name         = var.queue_name
  namespace_id = azurerm_servicebus_namespace.namespace.id
}

resource "azurerm_servicebus_namespace_authorization_rule" "app" {
  name         = "claims-app"
  namespace_id = azurerm_servicebus_namespace.namespace.id
  listen       = true
  send         = true
  manage       = false
}
