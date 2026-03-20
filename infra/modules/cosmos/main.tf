resource "azurerm_cosmosdb_account" "account" {
  name                = var.account_name
  resource_group_name = var.resource_group_name
  location            = var.location
  offer_type          = "Standard"
  kind                = "MongoDB"

  # Free tier: 1000 RU/s + 25 GB storage at no cost. One per subscription.
  free_tier_enabled = true

  capabilities {
    name = "EnableMongo"
  }

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }

  mongo_server_version = "6.0"
}

resource "azurerm_cosmosdb_mongo_database" "db" {
  name                = var.database_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.account.name
}
