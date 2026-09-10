using System.Text;
using ClyvoVetChallenge.Api.Exceptions;
using ClyvoVetChallenge.Api.Extensions;
using ClyvoVetChallenge.Api.Health;
using ClyvoVetChallenge.Api.Middlewares;
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

var customPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(customPort))
{
    builder.WebHost.UseUrls($"http://*:{customPort}");
}

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDBContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

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

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? Environment.GetEnvironmentVariable("JWT_KEY");
}
if (string.IsNullOrEmpty(jwtKey) && builder.Environment.IsDevelopment())
{
    jwtKey = "clyvovet_dev_jwt_secret_key_sprint_3_fiap_2026!";
}
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Chave JWT nao configurada. Defina a variavel de ambiente JWT_SECRET.");
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ClyvoVetApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ClyvoVetApi",
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
    .AddDbContextCheck<ClyvoVetContext>("database", failureStatus: HealthStatus.Unhealthy, tags: ["db", "ready"])
    .AddUrlGroup(new Uri("https://fiap.com.br"), name: "fiap-portal", failureStatus: HealthStatus.Degraded, tags: ["external", "ready"]);

var app = builder.Build();

Log.Information("ClyvoVet API iniciada com sucesso. Documentacao Swagger: http://localhost:5177/swagger");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowAll");
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:UseSqlite"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ClyvoVetContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClyvoVet API v1");
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
    try
    {
        var context = services.GetRequiredService<ClyvoVetContext>();
        if (!builder.Configuration.GetValue<bool>("Database:UseSqlite"))
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao aplicar migrations no banco de dados.");
        try
        {
            var context = services.GetRequiredService<ClyvoVetContext>();
            context.Database.EnsureCreated();
        }
        catch
        {
        }
    }
}

app.Run();

public partial class Program { }