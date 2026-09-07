using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class SintomaTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var sintoma = new Sintoma("Febre");

        Assert.Equal("Febre", sintoma.Nome);
        Assert.NotEqual(Guid.Empty, sintoma.Id);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarNome()
    {
        var sintoma = new Sintoma("Febre");

        sintoma.Update("Febre Alta");

        Assert.Equal("Febre Alta", sintoma.Nome);
    }
}
