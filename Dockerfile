# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY *.csproj .
RUN dotnet restore

# Copiar todo y publicar
COPY . .
RUN dotnet publish -c Release -o /app/out

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Escucha en el puerto dinámico asignado por Render
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "agromarketapi.dll"]
