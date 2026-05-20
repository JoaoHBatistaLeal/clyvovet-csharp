using System.ComponentModel.DataAnnotations;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class ClinicaRequest
{
    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    [StringLength(20)]
    public string Telefone { get; set; }

    [StringLength(255)]
    public string Endereco { get; set; }

    [StringLength(100)]
    public string HorarioFuncionamento { get; set; }

    public Clinica ToDomain() => new Clinica(Cnpj, Email, Telefone, Endereco, HorarioFuncionamento);
}