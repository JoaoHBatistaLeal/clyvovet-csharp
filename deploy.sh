#!/usr/bin/env bash
set -euo pipefail
export MSYS_NO_PATHCONV=1

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

REQUIRED_SUBSCRIPTION="2026-2TDSPJ-JOAO-H-BATISTA-LEAL-RM564361"
RESOURCE_GROUP="rg-bytecare"
LOCATION="brazilsouth"
ACR_NAME="acrbytecare"
DB_NAME="db_bytecare"
API_PORT="8080"

: "${DB_USER:=bytecare_admin}"
DB_PASSWORD="${DB_PASSWORD:-}"
JWT_SECRET="${JWT_SECRET:-}"
AUTH_USERNAME="${AUTH_USERNAME:-admin}"
AUTH_PASSWORD="${AUTH_PASSWORD:-}"
KEY_VAULT_NAME="${KEY_VAULT_NAME:-kv-bytecare-564361}"

# ==============================================================================
# FASE 1 & 2: VALIDACAO RIGOROSA DE SEGURANCA E VARIAVEIS SENSIVEIS
# ==============================================================================
if [ -z "$DB_PASSWORD" ] || [ -z "$JWT_SECRET" ]; then
  echo "================================================================================"
  echo "❌ ERRO DE SEGURANÇA: Credenciais obrigatórias não foram informadas!"
  echo "Por diretrizes de segurança da Sprint 3 (DevOps & Cloud), valores padrão"
  echo "e senhas hardcoded foram 100% eliminados deste script e do repositório."
  echo ""
  echo "Por favor, exporte as seguintes variáveis de ambiente antes de executar:"
  echo "  export DB_PASSWORD='sua_senha_forte_do_postgres'"
  echo "  export JWT_SECRET='seu_secret_jwt_com_pelo_menos_32_caracteres'"
  echo ""
  echo "Variáveis opcionais:"
  echo "  export DB_USER='bytecare_admin' (padrão: bytecare_admin)"
  echo "  export AUTH_USERNAME='admin' (padrão: admin)"
  echo "  export AUTH_PASSWORD='sua_senha_de_autenticacao_admin'"
  echo "  export KEY_VAULT_NAME='kv-bytecare-564361'"
  echo "================================================================================"
  exit 1
fi

if [ ${#JWT_SECRET} -lt 32 ]; then
  echo "❌ ERRO: A variável JWT_SECRET deve possuir no mínimo 32 caracteres (256 bits)."
  exit 1
fi

# ==============================================================================
# DEFINICAO DO CONTEXTO DA SUBSCRIPTION AZURE OBRIGATORIA
# ==============================================================================
echo "==> Definindo contexto da subscription Azure: $REQUIRED_SUBSCRIPTION..."
if az account set --subscription "$REQUIRED_SUBSCRIPTION" 2>/dev/null; then
  echo "Assinatura ativa: $REQUIRED_SUBSCRIPTION"
else
  echo "Aviso: Nao foi possivel alternar automaticamente para a assinatura $REQUIRED_SUBSCRIPTION."
  echo "Certifique-se de estar autenticado com permissao via 'az login'."
fi

# ==============================================================================
# GRUPO DE RECURSOS
# ==============================================================================
echo "==> Criando/verificando Resource Group: $RESOURCE_GROUP ($LOCATION)..."
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

# ==============================================================================
# AZURE KEY VAULT (PROVISIONAMENTO E ARMAZENAMENTO SEGURO DE SECRETS)
# ==============================================================================
DB_HOST="db-bytecare.${LOCATION}.azurecontainer.io"
CONNECTION_STRING="Host=${DB_HOST};Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"

echo "==> Criando/verificando Azure Key Vault: $KEY_VAULT_NAME..."
if ! az keyvault show --resource-group "$RESOURCE_GROUP" --name "$KEY_VAULT_NAME" >/dev/null 2>&1; then
  az keyvault create \
    --resource-group "$RESOURCE_GROUP" \
    --name "$KEY_VAULT_NAME" \
    --location "$LOCATION" \
    --sku standard \
    --retention-days 7
fi

KEY_VAULT_URI="https://${KEY_VAULT_NAME}.vault.azure.net/"

echo "==> Registrando segredos no Azure Key Vault..."
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "Jwt--Key" --value "$JWT_SECRET" >/dev/null 2>&1 || true
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "ConnectionStrings--DefaultConnection" --value "$CONNECTION_STRING" >/dev/null 2>&1 || true
if [ -n "$AUTH_PASSWORD" ]; then
  az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "Auth--Password" --value "$AUTH_PASSWORD" >/dev/null 2>&1 || true
fi

# ==============================================================================
# AZURE CONTAINER REGISTRY (ACR)
# ==============================================================================
echo "==> Criando/verificando Azure Container Registry: $ACR_NAME..."
az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACR_NAME" \
  --sku Basic \
  --admin-enabled true

ACR_LOGIN_SERVER=$(az acr show --name "$ACR_NAME" --query "loginServer" -o tsv)
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

echo "$ACR_PASSWORD" | docker login "$ACR_LOGIN_SERVER" -u "$ACR_USERNAME" --password-stdin

echo "==> Importando imagem PostgreSQL 16 para o ACR..."
az acr import \
  --name "$ACR_NAME" \
  --source docker.io/library/postgres:16-alpine \
  --image postgres-bytecare:v1 \
  --force

# ==============================================================================
# AZURE CONTAINER INSTANCES (ACI) - POSTGRESQL 16
# ==============================================================================
DB_STATE=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-db-bytecare --query "instanceView.state" -o tsv 2>/dev/null || echo "NotFound")
if [ "$DB_STATE" != "Running" ]; then
  echo "==> Criando container aci-db-bytecare com --secure-environment-variables..."
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
    --secure-environment-variables \
      POSTGRES_PASSWORD="$DB_PASSWORD"
else
  echo "Container aci-db-bytecare ja esta em execucao (Running). Preservando dados do banco."
fi

echo "==> Aguardando o banco de dados aci-db-bytecare iniciar..."
for i in $(seq 1 30); do
  DB_STATE=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-db-bytecare --query "instanceView.state" -o tsv 2>/dev/null || echo "Starting")
  if [ "$DB_STATE" = "Running" ]; then
    echo "Banco aci-db-bytecare esta Running!"
    break
  fi
  sleep 4
done

# ==============================================================================
# BUILD E PUSH DA IMAGEM DA API (.NET 9 / NÃO-ROOT)
# ==============================================================================
echo "==> Construindo e publicando imagem da API no ACR..."
docker build -t "${ACR_LOGIN_SERVER}/api-bytecare:v1" .
docker push "${ACR_LOGIN_SERVER}/api-bytecare:v1"

if az container show --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --query "name" -o tsv 2>/dev/null | grep -q "aci-api-bytecare"; then
  echo "Removendo container aci-api-bytecare anterior para deploy limpo da nova versao..."
  az container delete --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --yes >/dev/null 2>&1 || true
fi

# ==============================================================================
# AZURE CONTAINER INSTANCES (ACI) - API .NET COM VARIAVEIS SEGURAS
# ==============================================================================
echo "==> Provisionando container aci-api-bytecare com --secure-environment-variables..."
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
    DB_HOST="$DB_HOST" \
    DB_PORT="5432" \
    DB_NAME="$DB_NAME" \
    DB_USER="$DB_USER" \
    AUTH_USERNAME="$AUTH_USERNAME" \
    KEY_VAULT_NAME="$KEY_VAULT_NAME" \
    KEY_VAULT_URI="$KEY_VAULT_URI" \
  --secure-environment-variables \
    JWT_SECRET="$JWT_SECRET" \
    ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
    DB_PASSWORD="$DB_PASSWORD" \
    AUTH_PASSWORD="$AUTH_PASSWORD"

API_FQDN="api-bytecare.${LOCATION}.azurecontainer.io"

echo "==> Aguardando a API aci-api-bytecare iniciar..."
for i in $(seq 1 30); do
  API_STATE=$(az container show --resource-group "$RESOURCE_GROUP" --name aci-api-bytecare --query "instanceView.state" -o tsv 2>/dev/null || echo "Starting")
  if [ "$API_STATE" = "Running" ]; then
    echo "API aci-api-bytecare esta Running!"
    break
  fi
  sleep 5
done

az container list --resource-group "$RESOURCE_GROUP" --output table

echo ""
echo "=========================================="
echo "Endpoints ByteCare API:"
echo "Swagger:    http://${API_FQDN}:${API_PORT}/swagger"
echo "Health:     http://${API_FQDN}:${API_PORT}/health"
echo "Metrics:    http://${API_FQDN}:${API_PORT}/metrics"
echo "Key Vault:  ${KEY_VAULT_URI}"
echo "Database:   ${DB_HOST}:5432"
echo "=========================================="
echo ""

echo "Verificando se a API esta online e se comunicando com o banco..."
for i in $(seq 1 15); do
  HEALTH_RESP=$(curl -s "http://${API_FQDN}:${API_PORT}/health" || true)
  if echo "$HEALTH_RESP" | grep -qi '"status": *"Healthy"'; then
    echo "Sucesso! API esta online e conectada ao banco de dados:"
    echo "$HEALTH_RESP"
    break
  fi
  echo "Aguardando /health responder status Healthy (tentativa $i/15)..."
  sleep 5
done