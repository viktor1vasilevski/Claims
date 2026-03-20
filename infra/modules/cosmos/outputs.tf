output "connection_string" {
  sensitive = true
  value     = azurerm_cosmosdb_account.account.primary_mongodb_connection_string
}
