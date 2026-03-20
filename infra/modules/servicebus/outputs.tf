output "connection_string" {
  sensitive = true
  value     = azurerm_servicebus_namespace_authorization_rule.app.primary_connection_string
}

output "queue_name" {
  value = azurerm_servicebus_queue.audit_queue.name
}
