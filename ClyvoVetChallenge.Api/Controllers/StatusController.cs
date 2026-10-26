using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoVetChallenge.Api.Controllers;

[AllowAnonymous]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class StatusController(IHostEnvironment environment) : ControllerBase
{
    /// <summary>
    /// Endpoint de identificacao e status da aplicacao para monitoramento e validacao de pipeline CI/CD.
    /// </summary>
    /// <returns>Dados da aplicacao, versao da entrega e status operacional</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            application = "Clyvo Vet API",
            version = "1.0.0-sprint4",
            environment = environment.EnvironmentName,
            status = "Online",
            timestamp = DateTime.UtcNow
        });
    }
}
