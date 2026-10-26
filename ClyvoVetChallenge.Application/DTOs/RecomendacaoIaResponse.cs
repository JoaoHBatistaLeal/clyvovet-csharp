namespace ClyvoVetChallenge.Application.DTOs;

public class RecomendacaoIaResponse
{
    public string? Id { get; set; }
    public Guid AnimalId { get; set; }
    public string NomeAnimal { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string DiagnosticoSugerido { get; set; } = string.Empty;
    public string NivelUrgencia { get; set; } = string.Empty;
    public List<string> RecomendacoesCuidados { get; set; } = [];
    public DateTime DataAvaliacao { get; set; }
    public string ModeloIaUtilizado { get; set; } = string.Empty;
}
