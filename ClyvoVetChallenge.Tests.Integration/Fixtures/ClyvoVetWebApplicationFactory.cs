using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Tests.Integration.Fixtures;

public class ClyvoVetWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbFile = $"clyvovet-test-{Guid.NewGuid():N}.db";

    public ClyvoVetWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("Database__UseSqlite", "true");
        Environment.SetEnvironmentVariable("ConnectionStrings__ClyvoVetSqlite", $"Data Source={_dbFile}");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:UseSqlite"] = "true",
                ["ConnectionStrings:ClyvoVetSqlite"] = $"Data Source={_dbFile}"
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
