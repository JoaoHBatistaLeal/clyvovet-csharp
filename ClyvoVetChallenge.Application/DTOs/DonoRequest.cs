using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class DonoRequest
{
    [Required]
    [StringLength(150)]
    public string Nome { get; set; }

    [Required]
    [StringLength(14)]
    public string Cpf { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    [StringLength(20)]
    public string Telefone { get; set; }

    [Required]
    public DateOnly DataNascimento { get; set; }

    [StringLength(255)]
    public string Endereco { get; set; }

    public Dono ToDomain() => new Dono(Nome, Cpf, Email, Telefone, DataNascimento, Endereco);
}