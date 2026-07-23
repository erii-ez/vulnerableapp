# Usa la imagen del SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura las dependencias
COPY ["VulnerableApp.csproj", "./"]
RUN dotnet restore "VulnerableApp.csproj"

# Copia el resto del código y publica la aplicación
COPY . .
RUN dotnet publish "VulnerableApp.csproj" -c Release -o /app/publish

# Usa la imagen de ASP.NET (más ligera) para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "VulnerableApp.dll"]