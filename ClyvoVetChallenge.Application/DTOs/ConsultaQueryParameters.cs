using ClyvoVetChallenge.Application.DTOs.Common;

namespace ClyvoVetChallenge.Application.DTOs;

public class ConsultaQueryParameters : QueryParameters
{
    public Guid? AnimalId { get; set; }
    public Guid? VeterinarioId { get; set; }
    public Guid? ClinicaId { get; set; }
    public string? Status { get; set; }
    public string? Tipo { get; set; }
    public DateOnly? DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
}
