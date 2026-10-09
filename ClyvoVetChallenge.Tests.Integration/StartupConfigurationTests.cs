using ClyvoVetChallenge.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class StartupConfigurationTests
{
    [Fact]
    public void Startup_SemJwtConfigurado_DeveLancarInvalidOperationException()
    {
        try
        {
            Environment.SetEnvironmentVariable("Database__UseSqlite", "true");
            Environment.SetEnvironmentVariable("ConnectionStrings__ClyvoVetSqlite", "Data Source=test.db");
            Environment.SetEnvironmentVariable("Jwt__Key", "");
            Environment.SetEnvironmentVariable("JWT_SECRET", "");
            Environment.SetEnvironmentVariable("JWT_KEY", "");

            var factory = new WebApplicationFactory<Program>();
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = factory.CreateClient();
            });

            Assert.Contains("Jwt:Key", ex.Message);
            Assert.Contains("Azure Key Vault", ex.Message);
        }
        finally
        {
            ClyvoVetWebApplicationFactory.EnsureTestEnvironment();
        }
    }

    [Fact]
    public void Startup_ComJwtMenorQue32Caracteres_DeveLancarInvalidOperationException()
    {
        try
        {
            Environment.SetEnvironmentVariable("Database__UseSqlite", "true");
            Environment.SetEnvironmentVariable("ConnectionStrings__ClyvoVetSqlite", "Data Source=test.db");
            Environment.SetEnvironmentVariable("Jwt__Key", "short-jwt-key");
            Environment.SetEnvironmentVariable("JWT_SECRET", "short-jwt-key");

            var factory = new WebApplicationFactory<Program>();
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = factory.CreateClient();
            });

            Assert.Contains("32 caracteres", ex.Message);
        }
        finally
        {
            ClyvoVetWebApplicationFactory.EnsureTestEnvironment();
        }
    }

    [Fact]
    public void Startup_SemPostgreSqlConfiguradoEModoNaoSqlite_DeveLancarInvalidOperationException()
    {
        try
        {
            Environment.SetEnvironmentVariable("Database__UseSqlite", "false");
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "");
            Environment.SetEnvironmentVariable("Jwt__Key", "valid_secure_jwt_token_key_for_testing_purposes_only_32!");
            Environment.SetEnvironmentVariable("JWT_SECRET", "valid_secure_jwt_token_key_for_testing_purposes_only_32!");

            var factory = new WebApplicationFactory<Program>();
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = factory.CreateClient();
            });

            Assert.Contains("DefaultConnection", ex.Message);
            Assert.Contains("Azure Key Vault", ex.Message);
        }
        finally
        {
            ClyvoVetWebApplicationFactory.EnsureTestEnvironment();
        }
    }
}
