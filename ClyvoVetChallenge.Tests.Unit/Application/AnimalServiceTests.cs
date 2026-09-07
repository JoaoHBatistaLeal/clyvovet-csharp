using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class AnimalServiceTests
{
    private readonly Mock<IAnimalRepository> _repositoryMock = new();
    private readonly AnimalService _service;

    public AnimalServiceTests()
    {
        _service = new AnimalService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemRegistros_DeveRetornarColecaoDeAnimalResponse()
    {
        var animal = new Animal(Guid.NewGuid(), "Rex", "Canina", "Labrador", "Macho", new DateOnly(2021, 5, 10), 25.5m);
        _repositoryMock.Setup(r => r.GetAll()).Returns([animal]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Rex", resultado.First().Nome);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarAnimalResponse()
    {
        var animal = new Animal(Guid.NewGuid(), "Rex", "Canina", "Labrador", "Macho", new DateOnly(2021, 5, 10), 25.5m);
        _repositoryMock.Setup(r => r.GetById(animal.Id)).Returns(animal);

        var resultado = _service.GetById(animal.Id);

        Assert.NotNull(resultado);
        Assert.Equal(animal.Id, resultado.Id);
        Assert.Equal("Rex", resultado.Nome);
        _repositoryMock.Verify(r => r.GetById(animal.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Animal?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornarAnimalResponse()
    {
        var request = new AnimalRequest
        {
            DonoId = Guid.NewGuid(),
            Nome = "Thor",
            Especie = "Canina",
            Raca = "Pastor Alemao",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2022, 1, 1),
            Peso = 30.0m
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Thor", resultado.Nome);
        Assert.Equal(30.0m, resultado.Peso);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Animal>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizarERetornarAnimalResponse()
    {
        var animal = new Animal(Guid.NewGuid(), "Rex", "Canina", "Labrador", "Macho", new DateOnly(2021, 5, 10), 25.5m);
        var request = new AnimalRequest
        {
            Nome = "Rex Atualizado",
            Peso = 28.0m
        };
        _repositoryMock.Setup(r => r.GetById(animal.Id)).Returns(animal);

        var resultado = _service.Update(animal.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Rex Atualizado", resultado.Nome);
        Assert.Equal(28.0m, resultado.Peso);
        _repositoryMock.Verify(r => r.Update(animal), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new AnimalRequest { Nome = "Rex", Peso = 25m };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Animal?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var animal = new Animal(Guid.NewGuid(), "Rex", "Canina", "Labrador", "Macho", new DateOnly(2021, 5, 10), 25.5m);
        _repositoryMock.Setup(r => r.GetById(animal.Id)).Returns(animal);

        var resultado = _service.Delete(animal.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(animal), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Animal?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Animal>()), Times.Never);
    }
}
