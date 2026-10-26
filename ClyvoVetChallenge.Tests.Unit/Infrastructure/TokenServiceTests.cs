using ClyvoVetChallenge.Application.Configurations;
using ClyvoVetChallenge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ClyvoVetChallenge.Tests.Unit.Infrastructure;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_ComCredenciaisValidas_DeveGerarJwtValidoComClaims()
    {
        var settings = Options.Create(new JwtSettings
        {
            SecretKey = "clyvovet_test_jwt_secret_key_minimum_32_characters_long!",
            Issuer = "ByteCareApi",
            Audience = "ByteCareClient",
            ExpirationMinutes = 60
        });
        var config = new ConfigurationBuilder().Build();
        var tokenService = new TokenService(settings, config);

        var token = tokenService.GenerateToken("veterinario1", "Veterinario");

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(60, tokenService.ExpirationMinutes);
    }

    [Fact]
    public void GenerateToken_SemChave_DeveLancarInvalidOperationException()
    {
        var settings = Options.Create(new JwtSettings
        {
            SecretKey = "",
            Key = ""
        });
        var config = new ConfigurationBuilder().Build();
        var tokenService = new TokenService(settings, config);

        Assert.Throws<InvalidOperationException>(() => tokenService.GenerateToken("admin", "Admin"));
    }
}
