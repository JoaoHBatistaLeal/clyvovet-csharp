using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClyvoVetChallenge.Application.Configurations;
using ClyvoVetChallenge.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClyvoVetChallenge.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly string _signingKey;

    public int ExpirationMinutes => _jwtSettings.ExpirationMinutes > 0 ? _jwtSettings.ExpirationMinutes : 120;

    public TokenService(IOptions<JwtSettings> jwtOptions, IConfiguration configuration)
    {
        _jwtSettings = jwtOptions.Value;

        _signingKey = !string.IsNullOrWhiteSpace(_jwtSettings.SecretKey)
            ? _jwtSettings.SecretKey
            : (!string.IsNullOrWhiteSpace(_jwtSettings.Key)
                ? _jwtSettings.Key
                : configuration["Jwt:Key"]
                  ?? configuration["Jwt--Key"]
                  ?? configuration["JWT_SECRET"]
                  ?? configuration["JwtSecret"]
                  ?? configuration["JWT_KEY"]
                  ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                  ?? Environment.GetEnvironmentVariable("JWT_KEY")
                  ?? string.Empty);
    }

    public string GenerateToken(string username, string role, IEnumerable<Claim>? additionalClaims = null)
    {
        if (string.IsNullOrWhiteSpace(_signingKey))
        {
            throw new InvalidOperationException("Chave secreta JWT nao configurada no Azure Key Vault ou variaveis de ambiente.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_signingKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role)
        };

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            Issuer = string.IsNullOrWhiteSpace(_jwtSettings.Issuer) ? "ByteCareApi" : _jwtSettings.Issuer,
            Audience = string.IsNullOrWhiteSpace(_jwtSettings.Audience) ? "ByteCareClient" : _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
