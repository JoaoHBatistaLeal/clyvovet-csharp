using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Enums;

namespace ClyvoVetChallenge.Application.DTOs;

public class ConsultaResultadoRequest
{
    [StringLength(1000)]
    public string Diagnostico { get; set; }

    [StringLength(1000)]
    public string Observacoes { get; set; }

    [Required]
    public ConsultaStatus Status { get; set; }
}