#!/usr/bin/env bash
set -euo pipefail
export MSYS_NO_PATHCONV=1

RESOURCE_GROUP="rg-bytecare"
LOCATION="brazilsouth"
ACR_NAME="acrbytecare"
DB_NAME="db_bytecare"
API_PORT="8080"

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

ACR_LOGIN_SERVER=$(az acr show --name "$ACR_NAME" --query "loginServer" -o tsv)
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

echo "$ACR_PASSWORD" | docker login "$ACR_LOGIN_SERVER" -u "$ACR_USERNAME" --password-stdin

az acr import \
  --name "$ACR_NAME" \
  --source docker.io/library/postgres:16-alpine \
  --image postgres-bytecare:v1 \
  --force

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

sleep 20

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
    ConnectionStrings__DefaultConnection="Host=${DB_HOST};Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}" \
    DB_HOST="$DB_HOST" \
    DB_PORT="5432" \
    DB_NAME="$DB_NAME" \
    DB_USER="$DB_USER" \
    DB_PASSWORD="$DB_PASSWORD"

az container list --resource-group "$RESOURCE_GROUP" --output table

API_FQDN="api-bytecare.${LOCATION}.azurecontainer.io"
echo "API: http://${API_FQDN}:${API_PORT}/swagger"
echo "Database: ${DB_HOST}:5432"