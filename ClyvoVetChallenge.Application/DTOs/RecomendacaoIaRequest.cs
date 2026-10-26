using System.ComponentModel.DataAnnotations;

namespace ClyvoVetChallenge.Application.DTOs;

public class RecomendacaoIaRequest
{
    [Required]
    public Guid AnimalId { get; set; }

    [Required]
    [MaxLength(100)]
    public string NomeAnimal { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Especie { get; set; } = string.Empty;

    [Range(0, 30)]
    public int IdadeAnos { get; set; }

    [Required]
    [MinLength(1)]
    public List<string> Sintomas { get; set; } = [];

    public string? ObservacoesAdicionais { get; set; }
}
