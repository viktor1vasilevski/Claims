terraform {
  backend "azurerm" {
    resource_group_name = "rg-claims-tfstate"
    # Replace with the storage account name printed by infra/bootstrap/create-backend.sh
    storage_account_name = "stclaimstfstate3989"
    container_name       = "tfstate"
    key                  = "staging.terraform.tfstate"
    # Storage key is supplied via ARM_ACCESS_KEY env var in CI (TF_STATE_STORAGE_KEY secret)
  }
}
