using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ClyvoVetChallenge.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Realiza a autenticacao do usuario e gera o token JWT.
    /// As credenciais sao validadas estritamente contra o Azure Key Vault ou variaveis seguras de ambiente.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Requisicao invalida",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Usuario e senha sao obrigatorios."
            });
        }

        var expectedUsername = configuration["Auth:Username"]
            ?? configuration["Auth--Username"]
            ?? configuration["AUTH_USERNAME"]
            ?? Environment.GetEnvironmentVariable("AUTH_USERNAME")
            ?? "admin";

        var expectedPassword = configuration["Auth:Password"]
            ?? configuration["Auth--Password"]
            ?? configuration["AUTH_PASSWORD"]
            ?? Environment.GetEnvironmentVariable("AUTH_PASSWORD");

        if (string.IsNullOrWhiteSpace(expectedPassword))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Type = "about:blank",
                Title = "Autenticacao nao configurada",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "A senha administrativa nao foi configurada no Azure Key Vault ou variavel de ambiente. Login bloqueado por seguranca."
            });
        }

        if (request.Username != expectedUsername || request.Password != expectedPassword)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Nao autorizado",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Credenciais invalidas."
            });
        }

        var jwtKey = configuration["Jwt:Key"]
            ?? configuration["Jwt--Key"]
            ?? configuration["JWT_SECRET"]
            ?? configuration["JwtSecret"]
            ?? configuration["JWT_KEY"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? Environment.GetEnvironmentVariable("JWT_KEY");

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("Chave secreta JWT nao configurada no Azure Key Vault ou ambiente.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = configuration["Jwt:Issuer"] ?? "ByteCareApi",
            Audience = configuration["Jwt:Audience"] ?? "ByteCareClient",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token) });
    }

    /// <summary>
    /// Obtem informacoes do perfil autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("perfil")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Perfil()
    {
        return Ok(new
        {
            usuario = User.Identity?.Name,
            autenticado = true
        });
    }
}

public record LoginRequest(string Username, string Password);
