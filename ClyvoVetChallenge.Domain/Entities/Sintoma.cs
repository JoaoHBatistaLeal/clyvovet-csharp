using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Sintoma : BaseEntity
{
    public string Nome { get; private set; }

    public Sintoma(string nome)
    {
        Nome = nome;
    }

    public void Update(string nome)
    {
        Nome = nome;
    }

    protected Sintoma() { }
}