#!/bin/bash

# =========================
# CONFIGURAÇÕES
# =========================
RG=rgqmove
ACR_NAME=acrqmove
IMAGE_NAME=qmoveapi:v1
LOCATION=eastus

# =========================
# 1️⃣ Criar Resource Group
# =========================
echo "🔹 Criando Resource Group (se não existir)..."
az group create --name $RG --location $LOCATION &> /dev/null || echo "RG já existe"

# =========================
# 2️⃣ Criar/verificar ACR
# =========================
echo "🔹 Criando/verificando ACR..."
az acr show -n $ACR_NAME -g $RG &> /dev/null || az acr create -n $ACR_NAME -g $RG --sku Basic --admin-enabled true

# =========================
# 3️⃣ Login no ACR
# =========================
echo "🔹 Logando no ACR..."
az acr login --name $ACR_NAME

# =========================
# 4️⃣ Build da imagem da API
# =========================
echo "🔹 Buildando imagem da API..."
docker build -t $ACR_NAME.azurecr.io/$IMAGE_NAME .

# =========================
# 5️⃣ Push da imagem da API para o ACR
# =========================
echo "🔹 Enviando imagem da API para o ACR..."
docker push $ACR_NAME.azurecr.io/$IMAGE_NAME

# =========================
# 6️⃣ Puxar imagem MySQL, tag e enviar para o ACR
# =========================
echo "🔹 Subindo imagem MySQL para o ACR..."
docker pull mysql:8
docker tag mysql:8 $ACR_NAME.azurecr.io/mysql:8
docker push $ACR_NAME.azurecr.io/mysql:8

echo "✅ Build e push concluídos!"