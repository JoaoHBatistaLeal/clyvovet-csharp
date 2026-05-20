using System;
using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class AnimalRequest
{
    [Required]
    public Guid DonoId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; }

    [Required]
    [StringLength(50)]
    public string Especie { get; set; }

    [StringLength(50)]
    public string Raca { get; set; }

    [StringLength(20)]
    public string Sexo { get; set; }

    [Required]
    public DateOnly DataNascimento { get; set; }

    [Required]
    public decimal Peso { get; set; }

    public Animal ToDomain() => new Animal(DonoId, Nome, Especie, Raca, Sexo, DataNascimento, Peso);
}