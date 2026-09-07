using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class VeterinarioServiceTests
{
    private readonly Mock<IVeterinarioRepository> _repositoryMock = new();
    private readonly VeterinarioService _service;

    public VeterinarioServiceTests()
    {
        _service = new VeterinarioService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemVeterinarios_DeveRetornarColecao()
    {
        var vet = new Veterinario("Dr Roberto", "CRMV9999", "Clinica Geral", "11988880000", "roberto@vet.com");
        _repositoryMock.Setup(r => r.GetAll()).Returns([vet]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Dr Roberto", resultado.First().Nome);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarVeterinario()
    {
        var vet = new Veterinario("Dr Roberto", "CRMV9999", "Clinica Geral", "11988880000", "roberto@vet.com");
        _repositoryMock.Setup(r => r.GetById(vet.Id)).Returns(vet);

        var resultado = _service.GetById(vet.Id);

        Assert.NotNull(resultado);
        Assert.Equal(vet.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(vet.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Veterinario?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new VeterinarioRequest
        {
            Nome = "Dr Roberto",
            Crmv = "CRMV9999",
            Especialidade = "Clinica Geral",
            Telefone = "11988880000",
            Email = "roberto@vet.com"
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Dr Roberto", resultado.Nome);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Veterinario>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var vet = new Veterinario("Dr Roberto", "CRMV9999", "Clinica Geral", "11988880000", "roberto@vet.com");
        var request = new VeterinarioRequest
        {
            Telefone = "11977771111",
            Email = "novo.roberto@vet.com"
        };
        _repositoryMock.Setup(r => r.GetById(vet.Id)).Returns(vet);

        var resultado = _service.Update(vet.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("11977771111", resultado.Telefone);
        Assert.Equal("novo.roberto@vet.com", resultado.Email);
        _repositoryMock.Verify(r => r.Update(vet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new VeterinarioRequest { Telefone = "119", Email = "e@v.com" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Veterinario?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Veterinario>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var vet = new Veterinario("Dr Roberto", "CRMV9999", "Clinica Geral", "11988880000", "roberto@vet.com");
        _repositoryMock.Setup(r => r.GetById(vet.Id)).Returns(vet);

        var resultado = _service.Delete(vet.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(vet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Veterinario?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Veterinario>()), Times.Never);
    }
}
