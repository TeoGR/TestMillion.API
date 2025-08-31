# TestMillion.API — Guía de ejecución (Docker + .NET 9 + SQL Server + Firebase)

Este repositorio contiene una API en **.NET 9** (Clean-ish/por capas) con persistencia en **SQL Server 2022** y **carga/uso de Firebase** mediante credenciales de **Service Account**.  
El objetivo de este documento es que **cualquier evaluador** pueda ejecutar el proyecto **sin contratiempos**.

---

## 🧰 Tecnologías
- **.NET 9** (ASP.NET Core Web API)
- **SQL Server 2022** (contenedor oficial de Microsoft)
- **Docker & Docker Compose v2**
- **Firebase Admin SDK** (credenciales vía Service Account)
- **Swagger / OpenAPI** para exploración de endpoints
- Healthchecks expuestos

---

## 📁 Estructura del repositorio (resumen)
```
.
├─ TestMillion.API/               # Proyecto Web API (.NET 9)
├─ TestMillion.Services/          # Capa de servicios
├─ TestMillion.Persistence/       # Contexto de datos (EF/Core u ORM equivalente)
├─ TestMillion.Domain/            # Entidades del dominio
├─ TestMillion.Tests/             # Tests
├─ docker-compose.yml
├─ .env.template                  # Plantilla de variables de entorno
├─ secrets/                       # (NO versionado) aquí va el JSON de Firebase
└─ TestMillion.API.sln
```

> **Importante:** En el repositorio **no** se incluyen secretos (`secrets/` está ignorado).
> 
> El archivo de las credenciales de Firebase fue enviado por medio del correo electronico para ejecutar la funcionalidad completa.
> 
> Sin este archivo el codigo no va a levantar, por eso es **importante** añadir este archivo

---

## ✅ Requisitos previos
- **Windows / macOS / Linux** con Docker Desktop (o Docker Engine + Compose v2).
- (Opcional) **.NET 9 SDK** si desea ejecutar la API sin Docker.

---

## 🔐 Configuración de secretos (Firebase)
1. Cree la carpeta `secrets` en la **raíz** del repo (si no existe).
2. Copie su **Service Account JSON** dentro de `./secrets` con el nombre **`firebase_credentials.json`**:  
   ```
   ./secrets/firebase_credentials.json
   ```
3. ¡Listo! Docker Compose montará este archivo como **Docker Secret** dentro del contenedor en:  
   ```
   /run/secrets/firebase_credentials
   ```
   y la aplicación lo leerá usando la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS=/run/secrets/firebase_credentials`.

> **Seguridad**: `secrets/` está ignorado por git.
> 
> **IMPORTANTE:** Este archivo fue enviado por medio del correo electronico en el email de la entrega de la prueba
 
---

## ⚙️ Variables de entorno
El proyecto utiliza un archivo `.env` para algunas variables básicas.  
1. Copie la plantilla:
   ```bash
   cp .env.template .env
   ```
2. Abra `.env` y ajuste (si lo desea) los valores por defecto, por ejemplo:
   - `SA_PASSWORD` → contraseña del usuario `sa` en SQL Server (por defecto viene una segura para desarrollo).
   - `ASPNETCORE_ENVIRONMENT` → `Development` por defecto.

> El **string de conexión** a SQL Server en el contenedor apunta al servicio `db` mediante DNS interno de Docker.  
> No necesita modificarlo para ejecución con Compose.

---

## ▶️ Ejecución con Docker (recomendado)
Desde la **raíz** del repositorio:

```bash
docker compose up -d --build
```

Esto hará:
- Construir la imagen de la API (`api`).
- Levantar **SQL Server** (`db`).
- Montar el **secret** de Firebase en `/run/secrets/firebase_credentials`.
- Publicar la API en `http://localhost:8080`.

### Endpoints principales
- **Swagger / OpenAPI**: http://localhost:8080/swagger  
- **Healthcheck**: http://localhost:8080/Million/HealthCheck

### Conexión a la Base de Datos
- **Servidor**: `localhost,1433`
- **Usuario**: `sa`
- **Contraseña**: la definida en `.env` (`SA_PASSWORD`)
- **DB**: `TestMillionDb` (se crea en el primer arranque si así está configurado en el código)

> **Nota**: El `docker-compose.yml` utiliza un **healthcheck** para SQL Server con `sqlcmd`. En algunas etiquetas de la imagen, `sqlcmd` está en `/opt/mssql-tools18/bin/sqlcmd`; si su entorno difiere y aparece error de ruta, cambie a `/opt/mssql-tools/bin/sqlcmd` en el `docker-compose.yml` y vuelva a levantar.

### Verifique que el secreto se montó correctamente
```bash
docker compose exec api ls -l /run/secrets
docker compose exec api head -n 1 /run/secrets/firebase_credentials
```
(El segundo comando solo imprime la primera línea para validar presencia.)

---

## 🖥️ Ejecución local (sin Docker) — **opcional**
Si prefiere correr la API directamente con .NET (usando su SQL Server local o Docker):

1. Asegure que tiene **.NET 9 SDK**.
2. Defina la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS` apuntando a su JSON **local** (ruta absoluta):  
   - **Windows (PowerShell):**
     ```powershell
     $env:GOOGLE_APPLICATION_CREDENTIALS="C:\ruta\al\firebase_credentials.json"
     dotnet run --project .\TestMillion.API\TestMillion.API.csproj
     ```
   - **Linux/macOS (bash):**
     ```bash
     export GOOGLE_APPLICATION_CREDENTIALS=/ruta/al/firebase_credentials.json
     dotnet run --project ./TestMillion.API/TestMillion.API.csproj
     ```

3. La API quedará accesible (según el perfil/puerto configurado por su `launchSettings.json`).

> Si usa SQL Server en contenedor, mantenga `docker compose up db -d` para tener la DB disponible y apunte su connection string localmente a `localhost,1433`.

---

## 🔎 Problemas comunes y soluciones

### 1) `Firebase credentials not configured.`
- Asegúrese de que **existe** `./secrets/firebase_credentials.json` (Docker) o que `GOOGLE_APPLICATION_CREDENTIALS` apunta a un archivo válido (local).
- Verifique **permisos** del secreto en Docker. Nuestro compose monta el secreto con modo lectura. Si usa un usuario no-root dentro del contenedor y tuviera problemas, puede ajustar en `docker-compose.yml`:
  ```yaml
  secrets:
    - source: firebase_credentials
      target: firebase_credentials
      mode: 0444
  ```

### 2) Redirección a HTTPS dentro del contenedor
- Si en el código utiliza `app.UseHttpsRedirection()` pero **no** expone HTTPS en Docker, podría redirigir a un puerto no expuesto.
- La plantilla de Compose expone **HTTP 8080**. Mantenga la redirección deshabilitada en contenedor o exponga también el puerto HTTPS y configure Kestrel con certificado.

### 3) Healthcheck del contenedor `db` falla
- Ajuste la ruta de `sqlcmd` en el `docker-compose.yml` según su imagen:
  - `/opt/mssql-tools18/bin/sqlcmd` (usualmente en 2022-latest)
  - `/opt/mssql-tools/bin/sqlcmd` (variantes)
- Alternativa rápida de healthcheck si no dispone de `sqlcmd`:
  ```yaml
  test: ["CMD-SHELL", "bash -c 'echo > /dev/tcp/localhost/1433'"]
  ```

### 4) Puerto 1433 en uso
- Detenga instancias anteriores de SQL Server o cambie el mapeo de puerto en `docker-compose.yml` (p. ej. `"11433:1433"`).

### 5) CORS
- Los orígenes válidos se configuran en `appsettings*.json` bajo `Cors:AllowedOrigins`.  
  Recuerde **no** incluir la barra final; ej.: `http://localhost:8080`.

---

## 🧹 Comandos útiles
```bash
# Ver logs en vivo
docker compose logs -f api
docker compose logs -f db

# Entrar al contenedor de la API
docker compose exec api sh

# Reconstruir desde cero
docker compose down -v
docker compose up -d --build

# Listar y revisar secrets montados (en el contenedor)
docker compose exec api ls -l /run/secrets
```

---

## 📝 Notas finales
- Los archivos sensibles **no** están versionados (`.env`, `secrets/`), como corresponde.
- El evaluador **debe** proveer el JSON de Service Account para ejecutar la funcionalidad Firebase.
- La API publica **Swagger** y **Healthcheck** para facilitar la validación.

Si tiene alguna duda durante la ejecución, por favor revise **logs** del contenedor `api` y confirme la presencia del secreto montado en `/run/secrets/firebase_credentials`.
