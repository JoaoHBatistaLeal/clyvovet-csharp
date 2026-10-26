using ClyvoVetChallenge.Api.Hateoas;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoVetChallenge.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AnimalController(IAnimalService animalService, IDonoService donoService) : ControllerBase
{
    /// <summary>
    /// Lista todos os animais cadastrados com links HATEOAS.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AnimalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var items = animalService.GetAll();
        foreach (var item in items)
        {
            HateoasLinkBuilder.AddAnimalLinks(item, HttpContext);
        }
        return Ok(items);
    }

    /// <summary>
    /// Consulta paginada, ordenada e filtrada de animais com hipermidia HATEOAS.
    /// </summary>
    /// <param name="parameters">Parametros de paginacao, ordenacao e filtros</param>
    /// <param name="cancellationToken">Token de cancelamento da requisicao</param>
    /// <returns>Envelope paginado com itens e links HATEOAS</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<AnimalResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] AnimalQueryParameters parameters, CancellationToken cancellationToken)
    {
        var paged = await animalService.GetPagedAsync(parameters, cancellationToken);
        HateoasLinkBuilder.AddPagedAnimalLinks(paged, parameters, HttpContext);
        return Ok(paged);
    }

    /// <summary>
    /// Busca um animal específico pelo seu identificador único.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnimalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var item = animalService.GetById(id);
        if (item is null) return NotFound();
        HateoasLinkBuilder.AddAnimalLinks(item, HttpContext);
        return Ok(item);
    }

    /// <summary>
    /// Registra um novo animal no sistema.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnimalResponse), StatusCodes.Status201Created)]
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
        HateoasLinkBuilder.AddAnimalLinks(created, HttpContext);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza as informações de um animal existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AnimalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] AnimalRequest request)
    {
        var updated = animalService.Update(id, request);
        if (updated is null) return NotFound();
        HateoasLinkBuilder.AddAnimalLinks(updated, HttpContext);
        return Ok(updated);
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
