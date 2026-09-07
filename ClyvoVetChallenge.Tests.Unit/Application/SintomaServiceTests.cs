using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class SintomaServiceTests
{
    private readonly Mock<ISintomaRepository> _repositoryMock = new();
    private readonly SintomaService _service;

    public SintomaServiceTests()
    {
        _service = new SintomaService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemSintomas_DeveRetornarColecao()
    {
        var sintoma = new Sintoma("Febre");
        _repositoryMock.Setup(r => r.GetAll()).Returns([sintoma]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Febre", resultado.First().Nome);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarSintoma()
    {
        var sintoma = new Sintoma("Febre");
        _repositoryMock.Setup(r => r.GetById(sintoma.Id)).Returns(sintoma);

        var resultado = _service.GetById(sintoma.Id);

        Assert.NotNull(resultado);
        Assert.Equal(sintoma.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(sintoma.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Sintoma?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new SintomaRequest { Nome = "Vomito" };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Vomito", resultado.Nome);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Sintoma>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var sintoma = new Sintoma("Febre");
        var request = new SintomaRequest { Nome = "Febre Alta" };
        _repositoryMock.Setup(r => r.GetById(sintoma.Id)).Returns(sintoma);

        var resultado = _service.Update(sintoma.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Febre Alta", resultado.Nome);
        _repositoryMock.Verify(r => r.Update(sintoma), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new SintomaRequest { Nome = "Febre" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Sintoma?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Sintoma>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var sintoma = new Sintoma("Febre");
        _repositoryMock.Setup(r => r.GetById(sintoma.Id)).Returns(sintoma);

        var resultado = _service.Delete(sintoma.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(sintoma), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Sintoma?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Sintoma>()), Times.Never);
    }
}
