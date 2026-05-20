using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class SintomaRequest
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; }

    public Sintoma ToDomain() => new Sintoma(Nome);
}