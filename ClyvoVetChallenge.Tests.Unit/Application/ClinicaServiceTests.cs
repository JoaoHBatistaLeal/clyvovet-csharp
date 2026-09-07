using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class ClinicaServiceTests
{
    private readonly Mock<IClinicaRepository> _repositoryMock = new();
    private readonly ClinicaService _service;

    public ClinicaServiceTests()
    {
        _service = new ClinicaService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemClinicas_DeveRetornarColecao()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");
        _repositoryMock.Setup(r => r.GetAll()).Returns([clinica]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("12345678000199", resultado.First().Cnpj);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarClinica()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");
        _repositoryMock.Setup(r => r.GetById(clinica.Id)).Returns(clinica);

        var resultado = _service.GetById(clinica.Id);

        Assert.NotNull(resultado);
        Assert.Equal(clinica.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(clinica.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Clinica?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new ClinicaRequest
        {
            Cnpj = "12345678000199",
            Email = "teste@clinica.com",
            Telefone = "11999990000",
            Endereco = "Av Brasil, 500",
            HorarioFuncionamento = "24h"
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("12345678000199", resultado.Cnpj);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Clinica>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");
        var request = new ClinicaRequest
        {
            Email = "atualizado@clinica.com",
            Telefone = "11988881111",
            HorarioFuncionamento = "08:00 - 20:00"
        };
        _repositoryMock.Setup(r => r.GetById(clinica.Id)).Returns(clinica);

        var resultado = _service.Update(clinica.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("atualizado@clinica.com", resultado.Email);
        _repositoryMock.Verify(r => r.Update(clinica), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new ClinicaRequest { Email = "novo@clinica.com", Telefone = "119", HorarioFuncionamento = "24h" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Clinica?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Clinica>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var clinica = new Clinica("12345678000199", "contato@clinica.com", "11999998888", "Rua das Flores, 123", "08:00 - 18:00");
        _repositoryMock.Setup(r => r.GetById(clinica.Id)).Returns(clinica);

        var resultado = _service.Delete(clinica.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(clinica), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Clinica?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Clinica>()), Times.Never);
    }
}
