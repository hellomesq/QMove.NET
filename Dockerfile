# Base runtime .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build com SDK do .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Define a variável de ambiente PORT para o Render (opcional, mas deixa explícito)
ENV PORT=80
ENTRYPOINT ["dotnet", "MotoMonitoramento.dll"]
