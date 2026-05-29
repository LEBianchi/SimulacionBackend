# 1. Usamos el SDK de .NET para compilar el código en el servidor
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# CAMBIAR "TuProyecto" POR EL NOMBRE DE TU ARCHIVO .csproj
COPY ["SimulacionBackend.csproj", "./"]
RUN dotnet restore "SimulacionBackend.csproj"
COPY . .
RUN dotnet publish "SimulacionBackend.csproj" -c Release -o /app/publish

# 2. Usamos una versión súper liviana para correr la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Le avisamos a Render que escuche por el puerto 8080 (obligatorio en Render)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# CAMBIAR "TuProyecto.dll" POR EL NOMBRE DE TU PROYECTO
ENTRYPOINT ["dotnet", "SimulacionBackend.dll"]