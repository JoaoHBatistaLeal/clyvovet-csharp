using Microsoft.AspNetCore.Mvc;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ConsultaController(IConsultaService consultaService) : ControllerBase
{
    /// <summary>
    /// Lista todas as consultas cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(consultaService.GetAll());
    }

    /// <summary>
    /// Busca uma consulta específica pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = consultaService.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Busca todas as consultas vinculadas a um animal específico.
    /// </summary>
    [HttpGet("animal/{animalId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetByAnimalId(Guid animalId)
    {
        return Ok(consultaService.GetByAnimalId(animalId));
    }

    /// <summary>
    /// Registra uma nova consulta no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ConsultaRequest request)
    {
        var created = consultaService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza o resultado ou parecer clínico de uma consulta existente.
    /// </summary>
    [HttpPut("{id:guid}/resultado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateResultado(Guid id, [FromBody] ConsultaResultadoRequest request)
    {
        var updated = consultaService.UpdateResultado(id, request);
        return updated is null ? NotFound() : Ok(updated);
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