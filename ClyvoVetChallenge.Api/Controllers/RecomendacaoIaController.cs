using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoVetChallenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecomendacaoIaController(IRecomendacaoIaService recomendacaoIaService) : ControllerBase
{
    /// <summary>
    /// Gera uma recomendacao inteligente de cuidado veterinario (IA) e persiste no MongoDB.
    /// </summary>
    /// <param name="request">Dados do animal, idade e sintomas relatados</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao</param>
    /// <returns>Analise clinica, nivel de urgencia e cuidados recomendados</returns>
    [HttpPost("avaliar")]
    [ProducesResponseType(typeof(RecomendacaoIaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Avaliar([FromBody] RecomendacaoIaRequest request, CancellationToken cancellationToken)
    {
        var response = await recomendacaoIaService.GerarRecomendacaoAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Consulta o historico de recomendacoes de IA de um animal no MongoDB.
    /// </summary>
    /// <param name="animalId">Identificador unico do animal</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao</param>
    /// <returns>Colecao de recomendacoes historicas do animal</returns>
    [HttpGet("animal/{animalId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<RecomendacaoIaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterHistoricoPorAnimal(Guid animalId, CancellationToken cancellationToken)
    {
        var response = await recomendacaoIaService.ObterHistoricoPorAnimalAsync(animalId, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Consulta todo o historico de analises de IA persistidas no NoSQL.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da requisicao</param>
    /// <returns>Colecao completa de recomendacoes clinicas geradas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RecomendacaoIaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos(CancellationToken cancellationToken)
    {
        var response = await recomendacaoIaService.ObterTodosHistoricosAsync(cancellationToken);
        return Ok(response);
    }
}
