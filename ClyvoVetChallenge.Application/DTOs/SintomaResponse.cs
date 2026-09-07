using System;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class SintomaResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; }

    public SintomaResponse() { }

    public SintomaResponse(Sintoma sintoma)
    {
        Id = sintoma.Id;
        Nome = sintoma.Nome;
    }
}