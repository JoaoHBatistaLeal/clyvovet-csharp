using System;
using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class PrescricaoRequest
{
    [Required]
    public Guid ConsultaId { get; set; }

    [Required]
    [StringLength(150)]
    public string Medicamento { get; set; }

    [Required]
    [StringLength(50)]
    public string Dosagem { get; set; }

    [Required]
    [StringLength(50)]
    public string Frequencia { get; set; }

    [Required]
    public int DuracaoDias { get; set; }

    [StringLength(500)]
    public string Observacao { get; set; }

    [Required]
    public DateTime DataPrescricao { get; set; }

    public Prescricao ToDomain() => new Prescricao(ConsultaId, Medicamento, Dosagem, Frequencia, DuracaoDias, Observacao, DataPrescricao);
}