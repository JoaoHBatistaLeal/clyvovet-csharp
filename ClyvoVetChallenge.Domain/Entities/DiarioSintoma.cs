using System;
using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class DiarioSintoma : BaseEntity
{
    public Guid DiarioId { get; private set; }
    public Guid SintomaId { get; private set; }
    public string Intensidade { get; private set; }

    public Diario Diario { get; private set; }
    public Sintoma Sintoma { get; private set; }

    public DiarioSintoma(Guid diarioId, Guid sintomaId, string intensidade)
    {
        DiarioId = diarioId;
        SintomaId = sintomaId;
        Intensidade = intensidade;
    }

    public void Update(string intensidade)
    {
        Intensidade = intensidade;
    }

    protected DiarioSintoma() { }
}