# --- base de runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY TestMillion.API/TestMillion.API.csproj TestMillion.API/
COPY TestMillion.Services/TestMillion.Services.csproj TestMillion.Services/
COPY TestMillion.Persistence/TestMillion.Persistence.csproj TestMillion.Persistence/
COPY TestMillion.Domain/TestMillion.Domain.csproj TestMillion.Domain/

RUN dotnet restore TestMillion.API/TestMillion.API.csproj --nologo

COPY . .

WORKDIR /src/TestMillion.API

RUN dotnet publish TestMillion.API.csproj -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .
# No es necesario crear la carpeta, el volumen la montará.

ENTRYPOINT ["dotnet", "TestMillion.API.dll"]