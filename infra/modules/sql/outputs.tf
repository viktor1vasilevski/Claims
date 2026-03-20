output "connection_string" {
  sensitive = true
  value     = "Server=tcp:${azurerm_mssql_server.server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.db.name};Persist Security Info=False;User ID=${var.admin_username};Password=${var.admin_password};Encrypt=True;TrustServerCertificate=False;"
}
