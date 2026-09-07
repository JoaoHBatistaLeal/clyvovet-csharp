using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class DiarioTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var animalId = Guid.NewGuid();
        var data = new DateOnly(2026, 5, 20);
        var hora = new TimeOnly(9, 15);

        var diario = new Diario(animalId, data, hora, "Animal bem disposto hoje");

        Assert.Equal(animalId, diario.AnimalId);
        Assert.Equal(data, diario.DataRegistro);
        Assert.Equal(hora, diario.HoraRegistro);
        Assert.Equal("Animal bem disposto hoje", diario.ObservacaoLivre);
        Assert.Empty(diario.SintomasRegistrados);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarObservacaoLivre()
    {
        var diario = new Diario(Guid.NewGuid(), new DateOnly(2026, 5, 20), new TimeOnly(9, 15), "Animal quieto");

        diario.Update("Animal voltou a comer normalmente");

        Assert.Equal("Animal voltou a comer normalmente", diario.ObservacaoLivre);
    }
}
