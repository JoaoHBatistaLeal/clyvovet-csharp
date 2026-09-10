using Microsoft.AspNetCore.Mvc;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AnimalController(IAnimalService animalService, IDonoService donoService) : ControllerBase
{
    /// <summary>
    /// Lista todos os animais cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(animalService.GetAll());
    }

    /// <summary>
    /// Busca um animal específico pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = animalService.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Registra um novo animal no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Create([FromBody] AnimalRequest request)
    {
        if (request.DataNascimento > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return UnprocessableEntity(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Entidade improcessavel",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "A data de nascimento nao pode ser uma data futura."
            });
        }

        var dono = donoService.GetById(request.DonoId);
        if (dono is null)
        {
            return BadRequest(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Requisicao invalida",
                Status = StatusCodes.Status400BadRequest,
                Detail = $"O dono com identificador '{request.DonoId}' nao existe."
            });
        }

        var created = animalService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza as informações de um animal existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] AnimalRequest request)
    {
        var updated = animalService.Update(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Remove um animal do sistema.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var deleted = animalService.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}