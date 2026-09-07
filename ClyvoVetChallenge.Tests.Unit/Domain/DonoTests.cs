using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class DonoTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var dataNasc = new DateOnly(1990, 1, 15);

        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", dataNasc, "Av Paulista, 1000");

        Assert.Equal("Carlos Silva", dono.Nome);
        Assert.Equal("12345678901", dono.Cpf);
        Assert.Equal("carlos@email.com", dono.Email);
        Assert.Equal("11987654321", dono.Telefone);
        Assert.Equal(dataNasc, dono.DataNascimento);
        Assert.Equal("Av Paulista, 1000", dono.Endereco);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarCamposPermitidos()
    {
        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", new DateOnly(1990, 1, 15), "Av Paulista, 1000");

        dono.Update("Carlos Eduardo Silva", "carlosedu@email.com", "11911112222", "Rua Augusta, 500");

        Assert.Equal("Carlos Eduardo Silva", dono.Nome);
        Assert.Equal("carlosedu@email.com", dono.Email);
        Assert.Equal("11911112222", dono.Telefone);
        Assert.Equal("Rua Augusta, 500", dono.Endereco);
    }
}
