using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class ClinicaTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");

        Assert.Equal("12345678000199", clinica.Cnpj);
        Assert.Equal("contato@clinica.com", clinica.Email);
        Assert.Equal("11999998888", clinica.Telefone);
        Assert.Equal("Rua das Flores, 123", clinica.Endereco);
        Assert.Equal("08:00 - 18:00", clinica.HorarioFuncionamento);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarEmailTelefoneEHorario()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");

        clinica.Update("novo@clinica.com", "11988887777", "07:00 - 19:00");

        Assert.Equal("novo@clinica.com", clinica.Email);
        Assert.Equal("11988887777", clinica.Telefone);
        Assert.Equal("07:00 - 19:00", clinica.HorarioFuncionamento);
        Assert.Equal("12345678000199", clinica.Cnpj);
    }
}
