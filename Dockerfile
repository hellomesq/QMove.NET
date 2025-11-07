
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore MotoMonitoramento.csproj

# Copia todo o código
COPY . ./

# Build e publish
RUN dotnet publish MotoMonitoramento.csproj -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
ENV PORT=8080
WORKDIR /app

# Cria usuário não-root
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

# Dá permissão à pasta da aplicação para o novo usuário
RUN chown -R appuser:appgroup /app

# Copia build da stage anterior
COPY --from=build /app ./

# Muda para o usuário não-root
USER appuser

# Exposição da porta
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "MotoMonitoramento.dll"]
