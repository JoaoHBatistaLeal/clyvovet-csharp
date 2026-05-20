using System;
using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class HistoricoRequest
{
    [Required]
    public Guid AnimalId { get; set; }

    [Required]
    public Guid ConsultaId { get; set; }

    [Required]
    public DateTime DataRegistro { get; set; }

    [Required]
    [StringLength(500)]
    public string Descricao { get; set; }

    [Required]
    [StringLength(50)]
    public string TipoEvento { get; set; }

    [StringLength(500)]
    public string Resultado { get; set; }

    public Historico ToDomain() => new Historico(AnimalId, ConsultaId, DataRegistro, Descricao, TipoEvento, Resultado);
}