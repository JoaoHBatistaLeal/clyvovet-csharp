using Microsoft.AspNetCore.Mvc;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Api.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class DiarioController(IDiarioService diarioService) : ControllerBase
{
    /// <summary>
    /// Lista todas as entradas de diário cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(diarioService.GetAll());
    }

    /// <summary>
    /// Busca uma entrada de diário específica pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = diarioService.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Registra uma nova entrada de diário no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] DiarioRequest request)
    {
        var created = diarioService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma entrada de diário existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] DiarioRequest request)
    {
        var updated = diarioService.Update(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Remove uma entrada de diário do sistema.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = diarioService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
