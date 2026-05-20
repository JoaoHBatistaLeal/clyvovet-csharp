using System;
using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class ConsultaRequest
{
    [Required]
    public Guid AnimalId { get; set; }

    [Required]
    public Guid VeterinarioId { get; set; }

    [Required]
    public Guid ClinicaId { get; set; }

    [Required]
    public DateOnly DataConsulta { get; set; }

    [Required]
    public TimeOnly HoraConsulta { get; set; }

    [Required]
    [StringLength(500)]
    public string Motivo { get; set; }

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; }

    public Consulta ToDomain() => new Consulta(AnimalId, VeterinarioId, ClinicaId, DataConsulta, HoraConsulta, Motivo, Tipo);
}