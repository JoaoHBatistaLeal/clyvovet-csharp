using System;
using System.Collections.Generic;
using System.Linq;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class DiarioSintomaResponse
{
    public Guid SintomaId { get; set; }
    public string NomeSintoma { get; set; }
    public string Intensidade { get; set; }

    public DiarioSintomaResponse() { }

    public DiarioSintomaResponse(DiarioSintoma diarioSintoma)
    {
        SintomaId = diarioSintoma.SintomaId;
        NomeSintoma = diarioSintoma.Sintoma?.Nome ?? string.Empty;
        Intensidade = diarioSintoma.Intensidade;
    }
}

public class DiarioResponse
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public DateOnly DataRegistro { get; set; }
    public TimeOnly HoraRegistro { get; set; }
    public string ObservacaoLivre { get; set; }
    
    public List<DiarioSintomaResponse> Sintomas { get; set; } = new();

    public DiarioResponse() { }

    public DiarioResponse(Diario diario)
    {
        Id = diario.Id;
        AnimalId = diario.AnimalId;
        DataRegistro = diario.DataRegistro;
        HoraRegistro = diario.HoraRegistro;
        ObservacaoLivre = diario.ObservacaoLivre;
        Sintomas = diario.SintomasRegistrados.Select(x => new DiarioSintomaResponse(x)).ToList();
    }
}