#!/bin/bash
# Run this script ONCE manually before using Terraform.
# It creates the Azure Storage Account that holds the Terraform state file.
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Contributor access on your subscription
#
# Usage:
#   chmod +x create-backend.sh
#   ./create-backend.sh

set -e

RESOURCE_GROUP="rg-claims-tfstate"
STORAGE_ACCOUNT="stclaimstfstate$RANDOM"  # random suffix to ensure global uniqueness
CONTAINER_NAME="tfstate"
LOCATION="uksouth"

echo "Creating resource group: $RESOURCE_GROUP"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output none

echo "Creating storage account: $STORAGE_ACCOUNT"
az storage account create \
  --name "$STORAGE_ACCOUNT" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku Standard_LRS \
  --min-tls-version TLS1_2 \
  --allow-blob-public-access false \
  --output none

echo "Creating blob container: $CONTAINER_NAME"
az storage container create \
  --name "$CONTAINER_NAME" \
  --account-name "$STORAGE_ACCOUNT" \
  --output none

STORAGE_KEY=$(az storage account keys list \
  --account-name "$STORAGE_ACCOUNT" \
  --query '[0].value' \
  --output tsv)

echo ""
echo "=== Bootstrap complete ==="
echo ""
echo "1. Update infra/environments/staging/backend.tf with:"
echo "     storage_account_name = \"$STORAGE_ACCOUNT\""
echo ""
echo "2. Add this as a GitHub Actions secret named TF_STATE_STORAGE_KEY:"
echo "     $STORAGE_KEY"
echo ""
