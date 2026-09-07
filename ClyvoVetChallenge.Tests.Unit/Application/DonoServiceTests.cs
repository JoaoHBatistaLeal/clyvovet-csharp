using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class DonoServiceTests
{
    private readonly Mock<IDonoRepository> _repositoryMock = new();
    private readonly DonoService _service;

    public DonoServiceTests()
    {
        _service = new DonoService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemDonos_DeveRetornarColecao()
    {
        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", new DateOnly(1990, 1, 15), "Av Paulista, 1000");
        _repositoryMock.Setup(r => r.GetAll()).Returns([dono]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Carlos Silva", resultado.First().Nome);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarDono()
    {
        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", new DateOnly(1990, 1, 15), "Av Paulista, 1000");
        _repositoryMock.Setup(r => r.GetById(dono.Id)).Returns(dono);

        var resultado = _service.GetById(dono.Id);

        Assert.NotNull(resultado);
        Assert.Equal(dono.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(dono.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Dono?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new DonoRequest
        {
            Nome = "Carlos Silva",
            Cpf = "12345678901",
            Email = "carlos@email.com",
            Telefone = "11987654321",
            DataNascimento = new DateOnly(1990, 1, 15),
            Endereco = "Av Paulista, 1000"
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Carlos Silva", resultado.Nome);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Dono>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", new DateOnly(1990, 1, 15), "Av Paulista, 1000");
        var request = new DonoRequest
        {
            Nome = "Carlos Santos",
            Email = "carlossantos@email.com",
            Telefone = "11999991111",
            Endereco = "Rua Bela Cintra, 200"
        };
        _repositoryMock.Setup(r => r.GetById(dono.Id)).Returns(dono);

        var resultado = _service.Update(dono.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Carlos Santos", resultado.Nome);
        _repositoryMock.Verify(r => r.Update(dono), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new DonoRequest { Nome = "Carlos", Email = "c@e.com", Telefone = "11", Endereco = "Rua" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Dono?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Dono>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var dono = new Dono("Carlos Silva", "12345678901", "carlos@email.com", "11987654321", new DateOnly(1990, 1, 15), "Av Paulista, 1000");
        _repositoryMock.Setup(r => r.GetById(dono.Id)).Returns(dono);

        var resultado = _service.Delete(dono.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(dono), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Dono?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Dono>()), Times.Never);
    }
}
