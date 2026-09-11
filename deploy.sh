#!/usr/bin/env bash
set -euo pipefail
export MSYS_NO_PATHCONV=1

RESOURCE_GROUP="rg-bytecare"
LOCATION="brazilsouth"
ACR_NAME="acrbytecare"
DB_NAME="db_bytecare"
API_PORT="8080"

: "${DB_USER:=bytecare_admin}"
: "${DB_PASSWORD:=ByteK4re!}"
: "${JWT_SECRET:=clyvovet_dev_jwt_secret_key_sprint_3_fiap_2026!}"

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

echo "Aguardando o banco de dados aci-db-bytecare iniciar..."
for i in $(seq 1 30); do
  DB_STATE=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-db-bytecare --query "instanceView.state" -o tsv 2>/dev/null || echo "Starting")
  if [ "$DB_STATE" = "Running" ]; then
    echo "Banco aci-db-bytecare está Running!"
    break
  fi
  sleep 4
done

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

API_FQDN="api-bytecare.${LOCATION}.azurecontainer.io"

echo "Aguardando a API aci-api-bytecare iniciar..."
for i in $(seq 1 30); do
  API_STATE=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --query "instanceView.state" -o tsv 2>/dev/null || echo "Starting")
  if [ "$API_STATE" = "Running" ]; then
    echo "API aci-api-bytecare está Running!"
    break
  fi
  sleep 5
done

az container list --resource-group "$RESOURCE_GROUP" --output table

echo ""
echo "=========================================="
echo "Endpoints ByteCare API:"
echo "Swagger:  http://${API_FQDN}:${API_PORT}/swagger"
echo "Health:   http://${API_FQDN}:${API_PORT}/health"
echo "Metrics:  http://${API_FQDN}:${API_PORT}/metrics"
echo "Database: ${DB_HOST}:5432"
echo "=========================================="
echo ""

echo "Verificando se a API está online e se comunicando com o banco..."
for i in $(seq 1 15); do
  HEALTH_RESP=$(curl -s "http://${API_FQDN}:${API_PORT}/health" || true)
  if echo "$HEALTH_RESP" | grep -qi '"status": *"Healthy"'; then
    echo "Sucesso! API está online e conectada ao banco de dados:"
    echo "$HEALTH_RESP"
    break
  fi
  echo "Aguardando /health responder status Healthy (tentativa $i/15)..."
  sleep 5
done