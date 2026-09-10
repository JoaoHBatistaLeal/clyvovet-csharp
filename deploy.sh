#!/usr/bin/env bash
set -euo pipefail
export MSYS_NO_PATHCONV=1

RESOURCE_GROUP="rg-bytecare"
LOCATION="brazilsouth"
ACR_NAME="acrbytecare"
STORAGE_ACCOUNT_NAME="stbytecare"
SHARE_NAME="pgdata"
DB_NAME="db_bytecare"
API_PORT="8080"
JWT_SECRET="${JWT_SECRET:-clyvovet_dev_jwt_secret_key_sprint_3_fiap_2026!}"

: "${DB_PASSWORD:?Defina a variável DB_PASSWORD antes de executar}"
: "${DB_USER:?Defina a variável DB_USER antes de executar}"

az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACR_NAME" \
  --sku Basic \
  --admin-enabled true

az acr login --name "$ACR_NAME"

ACR_LOGIN_SERVER=$(az acr show --name "$ACR_NAME" --query "loginServer" -o tsv)
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

docker login "$ACR_LOGIN_SERVER" -u "$ACR_USERNAME" -p "$ACR_PASSWORD"

az storage account create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$STORAGE_ACCOUNT_NAME" \
  --location "$LOCATION" \
  --sku Standard_LRS

STORAGE_KEY=$(az storage account keys list \
  --resource-group "$RESOURCE_GROUP" \
  --account-name "$STORAGE_ACCOUNT_NAME" \
  --query "[0].value" -o tsv)

az storage share create \
  --name "$SHARE_NAME" \
  --account-name "$STORAGE_ACCOUNT_NAME" \
  --account-key "$STORAGE_KEY"

docker pull postgres:16-alpine
docker tag postgres:16-alpine "${ACR_LOGIN_SERVER}/postgres-bytecare:v1"
docker push "${ACR_LOGIN_SERVER}/postgres-bytecare:v1"

az container create \
  --resource-group "$RESOURCE_GROUP" \
  --name aci-db-bytecare \
  --image "${ACR_LOGIN_SERVER}/postgres-bytecare:v1" \
  --os-type Linux \
  --cpu 1 \
  --memory 1.5 \
  --registry-login-server "${ACR_LOGIN_SERVER}" \
  --registry-username "$ACR_USERNAME" \
  --registry-password "$ACR_PASSWORD" \
  --dns-name-label db-bytecare \
  --ports 5432 \
  --environment-variables \
    POSTGRES_DB="$DB_NAME" \
    POSTGRES_USER="$DB_USER" \
    POSTGRES_PASSWORD="$DB_PASSWORD"

DB_HOST="db-bytecare.${LOCATION}.azurecontainer.io"

sleep 30

docker build -t "${ACR_LOGIN_SERVER}/api-bytecare:v1" .
docker push "${ACR_LOGIN_SERVER}/api-bytecare:v1"

az container create \
  --resource-group "$RESOURCE_GROUP" \
  --name aci-api-bytecare \
  --image "${ACR_LOGIN_SERVER}/api-bytecare:v1" \
  --os-type Linux \
  --cpu 1 \
  --memory 1.5 \
  --registry-login-server "${ACR_LOGIN_SERVER}" \
  --registry-username "$ACR_USERNAME" \
  --registry-password "$ACR_PASSWORD" \
  --dns-name-label api-bytecare \
  --ports "$API_PORT" \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT="Production" \
    ASPNETCORE_HTTP_PORTS="$API_PORT" \
    PORT="$API_PORT" \
    JWT_SECRET="$JWT_SECRET" \
    ConnectionStrings__DefaultConnection="Host=${DB_HOST};Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}" \
    DB_HOST="$DB_HOST" \
    DB_PORT="5432" \
    DB_NAME="$DB_NAME" \
    DB_USER="$DB_USER" \
    DB_PASSWORD="$DB_PASSWORD"

az container list --resource-group "$RESOURCE_GROUP" --output table

API_FQDN=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --query "ipAddress.fqdn" -o tsv 2>/dev/null || echo "api-bytecare.${LOCATION}.azurecontainer.io")
API_IP=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --query "ipAddress.ip" -o tsv 2>/dev/null || echo "")

echo "============================================================================="
echo "ByteCare Deploy Concluído com Sucesso!"
echo "API FQDN: http://${API_FQDN}:${API_PORT}/swagger"
if [ -n "$API_IP" ]; then
  echo "API IP:   http://${API_IP}:${API_PORT}/swagger"
fi
echo "Database: ${DB_HOST}:5432"
echo "============================================================================="