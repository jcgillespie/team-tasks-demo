output "server_fqdn" {
  description = "SQL Server fully-qualified domain name"
  value       = azurerm_mssql_server.this.fully_qualified_domain_name
}

output "database_name" {
  description = "Database name"
  value       = azurerm_mssql_database.this.name
}

output "connection_string" {
  description = "ADO.NET SQL connection string"
  sensitive   = true
  value       = "Server=tcp:${azurerm_mssql_server.this.fully_qualified_domain_name},1433;Initial Catalog=teamtasks;User ID=${var.admin_login};Password=${var.admin_password};Encrypt=True;TrustServerCertificate=False;"
}
