using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Diario : BaseEntity
{
    public Guid AnimalId { get; private set; }
    public DateOnly DataRegistro { get; private set; }
    public TimeOnly HoraRegistro { get; private set; }
    public string ObservacaoLivre { get; private set; }

    public Animal Animal { get; private set; }
    public List<DiarioSintoma> SintomasRegistrados { get; private set; } = new();

    public Diario(Guid animalId, DateOnly dataRegistro, TimeOnly horaRegistro, string observacaoLivre)
    {
        AnimalId = animalId;
        DataRegistro = dataRegistro;
        HoraRegistro = horaRegistro;
        ObservacaoLivre = observacaoLivre;
    }

    public void Update(string observacaoLivre)
    {
        ObservacaoLivre = observacaoLivre;
    }

    protected Diario() { }
}