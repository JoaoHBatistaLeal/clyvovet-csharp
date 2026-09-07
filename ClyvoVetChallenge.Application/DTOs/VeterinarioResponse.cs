using System;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class VeterinarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Crmv { get; set; }
    public string Especialidade { get; set; }
    public string Telefone { get; set; }
    public string Email { get; set; }

    public VeterinarioResponse() { }

    public VeterinarioResponse(Veterinario veterinario)
    {
        Id = veterinario.Id;
        Nome = veterinario.Nome;
        Crmv = veterinario.Crmv;
        Especialidade = veterinario.Especialidade;
        Telefone = veterinario.Telefone;
        Email = veterinario.Email;
    }
}