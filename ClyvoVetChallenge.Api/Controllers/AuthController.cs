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
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        var expectedUsername = configuration["Auth:Username"] ?? Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "admin";
        var expectedPassword = configuration["Auth:Password"] ?? Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "admin123";

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

        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? Environment.GetEnvironmentVariable("JWT_KEY");
        }
        if (string.IsNullOrEmpty(jwtKey))
        {
            jwtKey = "clyvovet_dev_jwt_secret_key_sprint_3_fiap_2026!";
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
            Issuer = configuration["Jwt:Issuer"] ?? "ClyvoVetApi",
            Audience = configuration["Jwt:Audience"] ?? "ClyvoVetApi",
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
