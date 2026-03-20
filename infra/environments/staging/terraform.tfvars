# Non-secret staging values — safe to commit.
# Secrets (sql_admin_password) are passed via -var in CI using GitHub Actions secrets.

resource_group_name          = "rg-claims-staging"
location                     = "westeurope"
sql_admin_username           = "sqladmin"
existing_plan_name           = "ASP-WeatherForecastRG-a676"
existing_plan_resource_group = "WeatherForecastRG"
