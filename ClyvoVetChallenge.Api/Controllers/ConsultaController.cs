using ClyvoVetChallenge.Api.Hateoas;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoVetChallenge.Api.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ConsultaController(IConsultaService consultaService) : ControllerBase
{
    /// <summary>
    /// Lista todas as consultas cadastradas com links HATEOAS.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsultaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var items = consultaService.GetAll();
        foreach (var item in items)
        {
            HateoasLinkBuilder.AddConsultaLinks(item, HttpContext);
        }
        return Ok(items);
    }

    /// <summary>
    /// Consulta paginada, ordenada e filtrada de consultas com hipermidia HATEOAS.
    /// </summary>
    /// <param name="parameters">Parametros de paginacao, ordenacao e filtros</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao</param>
    /// <returns>Envelope paginado com itens e links HATEOAS</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<ConsultaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] ConsultaQueryParameters parameters, CancellationToken cancellationToken)
    {
        var paged = await consultaService.GetPagedAsync(parameters, cancellationToken);
        HateoasLinkBuilder.AddPagedConsultaLinks(paged, parameters, HttpContext);
        return Ok(paged);
    }

    /// <summary>
    /// Busca uma consulta específica pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = consultaService.GetById(id);
        if (item is null) return NotFound();
        HateoasLinkBuilder.AddConsultaLinks(item, HttpContext);
        return Ok(item);
    }

    /// <summary>
    /// Busca todas as consultas vinculadas a um animal específico.
    /// </summary>
    [HttpGet("animal/{animalId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ConsultaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByAnimalId(Guid animalId)
    {
        var items = consultaService.GetByAnimalId(animalId);
        foreach (var item in items)
        {
            HateoasLinkBuilder.AddConsultaLinks(item, HttpContext);
        }
        return Ok(items);
    }

    /// <summary>
    /// Registra uma nova consulta no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ConsultaRequest request)
    {
        var created = consultaService.Create(request);
        HateoasLinkBuilder.AddConsultaLinks(created, HttpContext);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza o resultado ou parecer clínico de uma consulta existente.
    /// </summary>
    [HttpPut("{id:guid}/resultado")]
    [ProducesResponseType(typeof(ConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateResultado(Guid id, [FromBody] ConsultaResultadoRequest request)
    {
        var updated = consultaService.UpdateResultado(id, request);
        if (updated is null) return NotFound();
        HateoasLinkBuilder.AddConsultaLinks(updated, HttpContext);
        return Ok(updated);
    }

    /// <summary>
    /// Remove uma consulta do sistema.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = consultaService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}

