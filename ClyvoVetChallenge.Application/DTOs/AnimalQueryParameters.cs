using ClyvoVetChallenge.Application.DTOs.Common;

namespace ClyvoVetChallenge.Application.DTOs;

public class AnimalQueryParameters : QueryParameters
{
    public string? Nome { get; set; }
    public string? Especie { get; set; }
    public string? Raca { get; set; }
    public string? Sexo { get; set; }
    public Guid? DonoId { get; set; }
}
