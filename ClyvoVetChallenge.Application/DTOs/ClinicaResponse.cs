using System;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class ClinicaResponse
{
    public Guid Id { get; set; }
    public string Cnpj { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string HorarioFuncionamento { get; set; }

    public ClinicaResponse() { }

    public ClinicaResponse(Clinica clinica)
    {
        Id = clinica.Id;
        Cnpj = clinica.Cnpj;
        Email = clinica.Email;
        Telefone = clinica.Telefone;
        Endereco = clinica.Endereco;
        HorarioFuncionamento = clinica.HorarioFuncionamento;
    }
}