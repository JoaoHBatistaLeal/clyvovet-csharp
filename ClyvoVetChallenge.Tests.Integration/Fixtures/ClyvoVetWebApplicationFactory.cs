using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Tests.Integration.Fixtures;

public class ClyvoVetWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbFile = $"clyvovet-test-{Guid.NewGuid():N}.db";
    public const string TestJwtKey = "clyvovet_integration_testing_jwt_secret_key_2026_min32chars!";
    public const string TestUsername = "admin";
    public const string TestPassword = "TestSecureAuthPassword2026!";

    public static void EnsureTestEnvironment(string? dbFile = null)
    {
        Environment.SetEnvironmentVariable("Database__UseSqlite", "true");
        if (!string.IsNullOrEmpty(dbFile))
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__ClyvoVetSqlite", $"Data Source={dbFile}");
        }
        Environment.SetEnvironmentVariable("Jwt__Key", TestJwtKey);
        Environment.SetEnvironmentVariable("JWT_SECRET", TestJwtKey);
        Environment.SetEnvironmentVariable("Auth__Username", TestUsername);
        Environment.SetEnvironmentVariable("Auth__Password", TestPassword);
        Environment.SetEnvironmentVariable("AUTH_PASSWORD", TestPassword);
    }

    public ClyvoVetWebApplicationFactory()
    {
        EnsureTestEnvironment(_dbFile);
    }

    public static string GenerateTestToken(string username = TestUsername, string role = "Admin")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TestJwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = "ByteCareApi",
            Audience = "ByteCareClient",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public HttpClient CreateAuthenticatedClient(string username = TestUsername, string role = "Admin")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateTestToken(username, role));
        return client;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        EnsureTestEnvironment(_dbFile);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        EnsureTestEnvironment(_dbFile);
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:UseSqlite"] = "true",
                ["ConnectionStrings:ClyvoVetSqlite"] = $"Data Source={_dbFile}",
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = "ByteCareApi",
                ["Jwt:Audience"] = "ByteCareClient",
                ["Auth:Username"] = TestUsername,
                ["Auth:Password"] = TestPassword
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(DbContextOptions)).ToList();

            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={_dbFile}");
                options.UseInternalServiceProvider(
                    new ServiceCollection()
                        .AddEntityFrameworkSqlite()
                        .BuildServiceProvider());
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_dbFile))
        {
            try
            {
                File.Delete(_dbFile);
            }
            catch
            {
            }
        }
    }
}
