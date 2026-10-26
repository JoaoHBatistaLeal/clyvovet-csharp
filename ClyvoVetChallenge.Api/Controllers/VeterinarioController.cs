using Microsoft.AspNetCore.Mvc;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Api.Controllers;

using Microsoft.AspNetCore.Authorization;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class VeterinarioController(IVeterinarioService veterinarioService) : ControllerBase
{
    /// <summary>
    /// Lista todos os veterinários cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(veterinarioService.GetAll());
    }

    /// <summary>
    /// Busca um veterinário específico pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = veterinarioService.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Registra um novo veterinário no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] VeterinarioRequest request)
    {
        var created = veterinarioService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza as informações de um veterinário existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] VeterinarioRequest request)
    {
        var updated = veterinarioService.Update(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Remove um veterinário do sistema.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = veterinarioService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
