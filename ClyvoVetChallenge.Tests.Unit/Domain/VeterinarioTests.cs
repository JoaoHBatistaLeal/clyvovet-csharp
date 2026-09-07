using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class VeterinarioTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var vet = new Veterinario("Dra Ana", "CRMV12345", "Cirurgia", "11977776666", "ana@vet.com");

        Assert.Equal("Dra Ana", vet.Nome);
        Assert.Equal("CRMV12345", vet.Crmv);
        Assert.Equal("Cirurgia", vet.Especialidade);
        Assert.Equal("11977776666", vet.Telefone);
        Assert.Equal("ana@vet.com", vet.Email);
    }

    [Fact]
    public void UpdateContato_ComNovosDados_DeveAtualizarTelefoneEEmail()
    {
        var vet = new Veterinario("Dra Ana", "CRMV12345", "Cirurgia", "11977776666", "ana@vet.com");

        vet.UpdateContato("11955554444", "dra.ana@vet.com");

        Assert.Equal("11955554444", vet.Telefone);
        Assert.Equal("dra.ana@vet.com", vet.Email);
        Assert.Equal("CRMV12345", vet.Crmv);
    }
}
