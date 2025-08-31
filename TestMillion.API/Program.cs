using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using TestMillion.Persistence.Context;
using TestMillion.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

GoogleCredential? firebaseCredential = null;

var firebaseCredentialJson =
    builder.Configuration["Firebase:CredentialJson"]
    ?? Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_JSON");

if (!string.IsNullOrWhiteSpace(firebaseCredentialJson))
    firebaseCredential = GoogleCredential.FromJson(firebaseCredentialJson);

if (firebaseCredential is null)
{
    var firebaseCredentialPath =
        builder.Configuration["Firebase:CredentialsPath"]
        ?? Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

    if (!string.IsNullOrWhiteSpace(firebaseCredentialPath))
    {
        try
        {
            firebaseCredential = GoogleCredential.FromFile(firebaseCredentialPath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Firebase] No se pudo leer el archivo de credenciales en '{firebaseCredentialPath}': {ex.Message}");
        }
    }
}

if (firebaseCredential is null)
{
    try
    {
        firebaseCredential = await GoogleCredential.GetApplicationDefaultAsync();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[Firebase] ADC no disponible: {ex.Message}");
    }
}

if (firebaseCredential is null)
    throw new InvalidOperationException("Firebase credentials not configured.");

FirebaseApp.Create(new AppOptions { Credential = firebaseCredential });

var version = "v1";
var appName = "Million";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(version, new OpenApiInfo
    {
        Title = appName,
        Version = version,
        Description = "API para la prueba tecnica en Million - Mateo Garzon Restrepo",
    });
});

builder.Services.AddApplicationServices(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowFrontend");

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{appName} {version}");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseHealthChecks($"/{appName}/HealthCheck", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
