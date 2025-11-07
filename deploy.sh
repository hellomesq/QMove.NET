
#!/bin/bash

# =========================
# CONFIGURAÇÕES
# =========================
RG=rgqmove
ACR_NAME=acrqmove
APP_NAME=aci-qmove-api
DB_NAME=aci-qmove-db
LOCATION=eastus
IMAGE_NAME=qmoveapi:v1

MYSQL_ROOT_PASSWORD=root123
DB_DATABASE=qmove

# =========================
# 1️⃣ Criar Resource Group
# =========================
echo "🔹 Criando Resource Group..."
az group create --name $RG --location $LOCATION &> /dev/null || echo "RG já existe"

# =========================
# 2️⃣ Garantir que o ACR existe e pegar credenciais
# =========================
echo "🔹 Verificando ACR..."
az acr show -n $ACR_NAME -g $RG &> /dev/null || az acr create -n $ACR_NAME -g $RG --sku Basic --admin-enabled true

ACR_USER=$(az acr credential show -n $ACR_NAME --query "username" -o tsv)
ACR_PASS=$(az acr credential show -n $ACR_NAME --query "passwords[0].value" -o tsv)

# =========================
# 3️⃣ Criar container MySQL se não existir
# =========================
echo "🔹 Verificando container MySQL..."
DB_EXISTS=$(az container show --resource-group $RG --name $DB_NAME --query "name" -o tsv 2>/dev/null)

if [ -z "$DB_EXISTS" ]; then
    echo "⚡ Criando container MySQL..."
    az container create \
      --resource-group $RG \
      --name $DB_NAME \
      --image $ACR_NAME.azurecr.io/mysql:8 \
      --registry-login-server $ACR_NAME.azurecr.io \
      --registry-username $ACR_USER \
      --registry-password $ACR_PASS \
      --environment-variables MYSQL_ROOT_PASSWORD=$MYSQL_ROOT_PASSWORD MYSQL_DATABASE=$DB_DATABASE \
      --cpu 1 \
      --memory 1 \
      --ports 3306 \
      --os-type Linux \
      --dns-name-label $DB_NAME \
      --restart-policy Always

    echo "⌛ Aguardando container MySQL iniciar..."
    sleep 20
else
    echo "✅ Container MySQL já existe."
fi

# =========================
# 4️⃣ Pegar FQDN do container MySQL
# =========================
echo "🔹 Obtendo FQDN do MySQL..."
DB_FQDN=$(az container show --resource-group $RG --name $DB_NAME --query "ipAddress.fqdn" -o tsv)

if [ -z "$DB_FQDN" ]; then
    echo "❌ Não foi possível obter o FQDN do MySQL."
    exit 1
fi

echo "✅ Banco MySQL rodando em: $DB_FQDN"

# =========================
# 5️⃣ Criar container da aplicação
# =========================
echo "🔹 Criando container da aplicação..."
az container create \
  --resource-group $RG \
  --name $APP_NAME \
  --image $ACR_NAME.azurecr.io/$IMAGE_NAME \
  --registry-login-server $ACR_NAME.azurecr.io \
  --registry-username $ACR_USER \
  --registry-password $ACR_PASS \
  --cpu 1 --memory 1 \
  --os-type Linux \
  --ports 8080 \
  --environment-variables \
      DB_HOST=$DB_FQDN \
      DB_PORT=3306 \
      DB_NAME=$DB_DATABASE \
      DB_USER=root \
      DB_PASS=$MYSQL_ROOT_PASSWORD \
      PORT=8080 \
  --dns-name-label $APP_NAME \
  --restart-policy Always

echo "✅ Deploy concluído!"
echo "🌍 API disponível em: http://$APP_NAME.$LOCATION.azurecontainer.io"
