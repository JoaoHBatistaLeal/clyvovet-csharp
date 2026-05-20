using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class VeterinarioRequest
{
    [Required]
    [StringLength(150)]
    public string Nome { get; set; }

    [Required]
    [StringLength(20)]
    public string Crmv { get; set; }

    [StringLength(100)]
    public string Especialidade { get; set; }

    [StringLength(20)]
    public string Telefone { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    public Veterinario ToDomain() => new Veterinario(Nome, Crmv, Especialidade, Telefone, Email);
}