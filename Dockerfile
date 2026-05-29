# 1. Usamos el SDK de .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos absolutamente todo el repositorio al contenedor
COPY . .

# LE APUNTAMOS DIRECTO AL PROYECTO (Ignoramos la solución)
RUN dotnet publish SimulacionBackend/SimulacionBackend.csproj -c Release -o /app/publish

# 2. Usamos el entorno liviano para correr la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Puerto para Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Arrancamos la API
ENTRYPOINT ["dotnet", "SimulacionBackend.dll"]