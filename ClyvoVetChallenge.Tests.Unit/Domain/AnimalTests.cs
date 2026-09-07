using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class AnimalTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var donoId = Guid.NewGuid();
        var dataNascimento = new DateOnly(2021, 5, 10);

        var animal = new Animal(donoId, "Rex", "Canina", "Labrador", "Macho", dataNascimento, 25.5m);

        Assert.Equal(donoId, animal.DonoId);
        Assert.Equal("Rex", animal.Nome);
        Assert.Equal("Canina", animal.Especie);
        Assert.Equal("Labrador", animal.Raca);
        Assert.Equal("Macho", animal.Sexo);
        Assert.Equal(dataNascimento, animal.DataNascimento);
        Assert.Equal(25.5m, animal.Peso);
        Assert.NotEqual(Guid.Empty, animal.Id);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarNomeEPeso()
    {
        var animal = new Animal(Guid.NewGuid(), "Rex", "Canina", "Labrador", "Macho", new DateOnly(2021, 5, 10), 25.5m);

        animal.Update("Max", 27.0m);

        Assert.Equal("Max", animal.Nome);
        Assert.Equal(27.0m, animal.Peso);
    }
}
