using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class DiarioSintomaDto
{
    [Required]
    public Guid SintomaId { get; set; }

    [Required]
    [StringLength(50)]
    public string Intensidade { get; set; }
}

public class DiarioRequest
{
    [Required]
    public Guid AnimalId { get; set; }

    [Required]
    public DateOnly DataRegistro { get; set; }

    [Required]
    public TimeOnly HoraRegistro { get; set; }

    [StringLength(1000)]
    public string ObservacaoLivre { get; set; }

    public List<DiarioSintomaDto> Sintomas { get; set; } = new();

    public Diario ToDomain() => new Diario(AnimalId, DataRegistro, HoraRegistro, ObservacaoLivre);
}