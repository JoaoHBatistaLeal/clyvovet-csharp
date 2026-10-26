namespace ClyvoVetChallenge.Domain.Entities;

public class HistoricoRecomendacaoIa
{
    public string Id { get; set; } = string.Empty;
    public Guid AnimalId { get; set; }
    public string NomeAnimal { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public int IdadeAnos { get; set; }
    public List<string> SintomasRelatados { get; set; } = [];
    public string DiagnosticoSugerido { get; set; } = string.Empty;
    public string NivelUrgencia { get; set; } = string.Empty;
    public List<string> RecomendacoesCuidados { get; set; } = [];
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;
    public string ModeloIaUtilizado { get; set; } = "ClyvoVet-ClinicalAI-v2";
}
