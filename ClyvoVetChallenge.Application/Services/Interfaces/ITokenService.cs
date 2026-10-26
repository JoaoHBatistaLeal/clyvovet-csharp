using System.Security.Claims;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(string username, string role, IEnumerable<Claim>? additionalClaims = null);
    int ExpirationMinutes { get; }
}
