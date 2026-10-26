using System.Text;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using ClyvoVetChallenge.Api.Exceptions;
using ClyvoVetChallenge.Api.Extensions;
using ClyvoVetChallenge.Api.Health;
using ClyvoVetChallenge.Api.Middlewares;
using ClyvoVetChallenge.Application.Extensions;
using ClyvoVetChallenge.Infrastructure.Extensions;
using ClyvoVetChallenge.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

Env.TraversePath().Load();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ClyvoVetChallenge.Api")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/clyvovet-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KEY_VAULT_URI"]
    ?? Environment.GetEnvironmentVariable("KEY_VAULT_URI")
    ?? builder.Configuration["KeyVault:VaultUri"];

if (string.IsNullOrEmpty(keyVaultUri))
{
    var keyVaultName = builder.Configuration["KEY_VAULT_NAME"]
        ?? Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
    if (!string.IsNullOrEmpty(keyVaultName))
    {
        keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";
    }
}

if (!string.IsNullOrEmpty(keyVaultUri))
{
    try
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
        Log.Information("Azure Key Vault configurado com sucesso a partir de {VaultUri}", keyVaultUri);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Nao foi possivel carregar segredos do Azure Key Vault ({VaultUri}). Continuando com variaveis de ambiente.", keyVaultUri);
    }
}

var customPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(customPort))
{
    builder.WebHost.UseUrls($"http://*:{customPort}");
}

builder.Host.UseSerilog();

var useSqlite = builder.Configuration.GetValue<bool>("Database:UseSqlite");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
    ?? builder.Configuration["ConnectionStrings--DefaultConnection"]
    ?? builder.Configuration["DbConnectionString"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    var dbHost = builder.Configuration["DB_HOST"] ?? Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = builder.Configuration["DB_PORT"] ?? Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var dbName = builder.Configuration["DB_NAME"] ?? Environment.GetEnvironmentVariable("DB_NAME") ?? "db_bytecare";
    var dbUser = builder.Configuration["DB_USER"] ?? Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
    var dbPass = builder.Configuration["DB_PASSWORD"] ?? Environment.GetEnvironmentVariable("DB_PASSWORD");

    if (!string.IsNullOrWhiteSpace(dbHost) && !string.IsNullOrWhiteSpace(dbPass))
    {
        var csBuilder = new System.Data.Common.DbConnectionStringBuilder
        {
            ["Host"] = dbHost,
            ["Port"] = dbPort,
            ["Database"] = dbName,
            ["Username"] = dbUser,
            ["Password"] = dbPass
        };
        connectionString = csBuilder.ConnectionString;
        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    }
}

if (!useSqlite && string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A string de conexao com o banco de dados PostgreSQL ('ConnectionStrings:DefaultConnection') nao foi configurada. " +
        "Configure a conexao via Azure Key Vault ('ConnectionStrings--DefaultConnection') ou variavel de ambiente ('ConnectionStrings__DefaultConnection').");
}

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerCustom();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? builder.Configuration["Jwt--Key"]
    ?? builder.Configuration["JWT_SECRET"]
    ?? builder.Configuration["JwtSecret"]
    ?? builder.Configuration["JWT_KEY"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? Environment.GetEnvironmentVariable("JWT_KEY");

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "A chave secreta JWT ('Jwt:Key') nao foi configurada. " +
        "Configure o segredo via Azure Key Vault ('Jwt--Key' ou 'JwtSecret') ou variavel de ambiente ('JWT_SECRET').");
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "A chave JWT configurada deve possuir no minimo 32 caracteres (256 bits) para garantir assinatura criptografica segura com HmacSha256.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ByteCareApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ByteCareClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("ClyvoVetChallenge.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Routing",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http",
                "System.Runtime")
            .AddPrometheusExporter();
    });

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", failureStatus: HealthStatus.Unhealthy, tags: ["db", "ready"])
    .AddUrlGroup(new Uri("https://fiap.com.br"), name: "fiap-portal", failureStatus: HealthStatus.Degraded, tags: ["external", "ready"]);

var app = builder.Build();

Log.Information("ByteCare API iniciada com sucesso. Documentacao Swagger: http://localhost:8080/swagger");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowAll");
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ByteCare API v1");
    c.RoutePrefix = "swagger"; 
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.IsNpgsql())
        {
            var maxRetries = 10;
            for (var retry = 1; retry <= maxRetries; retry++)
            {
                try
                {
                    logger.LogInformation("Tentando aplicar migrations no PostgreSQL (tentativa {Retry}/{MaxRetries})...", retry, maxRetries);
                    context.Database.Migrate();
                    logger.LogInformation("Migrations aplicadas com sucesso no PostgreSQL.");
                    break;
                }
                catch (Exception ex) when (retry < maxRetries)
                {
                    logger.LogWarning(ex, "Falha ao conectar ou aplicar migrations no PostgreSQL (tentativa {Retry}/{MaxRetries}). Aguardando 3s...", retry, maxRetries);
                    Thread.Sleep(3000);
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao aplicar migrations no PostgreSQL durante a inicialização.");
    }
}

app.Run();

public partial class Program { }