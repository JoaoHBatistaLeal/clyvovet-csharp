using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Enums;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _repositoryMock = new();
    private readonly ConsultaService _service;

    public ConsultaServiceTests()
    {
        _service = new ConsultaService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemConsultas_DeveRetornarColecaoDeConsultaResponse()
    {
        var consulta = new Consulta(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial");
        _repositoryMock.Setup(r => r.GetAll()).Returns([consulta]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Vacina", resultado.First().Motivo);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarConsultaResponse()
    {
        var consulta = new Consulta(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial");
        _repositoryMock.Setup(r => r.GetById(consulta.Id)).Returns(consulta);

        var resultado = _service.GetById(consulta.Id);

        Assert.NotNull(resultado);
        Assert.Equal(consulta.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(consulta.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Consulta?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void GetByAnimalId_QuandoExistemConsultasParaAnimal_DeveRetornarListaDeConsultas()
    {
        var animalId = Guid.NewGuid();
        var consulta = new Consulta(animalId, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial");
        _repositoryMock.Setup(r => r.GetByAnimalId(animalId)).Returns([consulta]);

        var resultado = _service.GetByAnimalId(animalId);

        Assert.Single(resultado);
        Assert.Equal(animalId, resultado.First().AnimalId);
        _repositoryMock.Verify(r => r.GetByAnimalId(animalId), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornarConsulta()
    {
        var request = new ConsultaRequest
        {
            AnimalId = Guid.NewGuid(),
            VeterinarioId = Guid.NewGuid(),
            ClinicaId = Guid.NewGuid(),
            DataConsulta = new DateOnly(2026, 7, 1),
            HoraConsulta = new TimeOnly(9, 0),
            Motivo = "Checkup Geral",
            Tipo = "Presencial"
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Checkup Geral", resultado.Motivo);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Consulta>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void UpdateResultado_QuandoConsultaExiste_DeveAtualizarERetornarConsulta()
    {
        var consulta = new Consulta(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial");
        var request = new ConsultaResultadoRequest
        {
            Diagnostico = "Animal saudavel",
            Observacoes = "Vacina aplicada",
            Status = ConsultaStatus.Concluida
        };
        _repositoryMock.Setup(r => r.GetById(consulta.Id)).Returns(consulta);

        var resultado = _service.UpdateResultado(consulta.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Animal saudavel", resultado.Diagnostico);
        Assert.Equal(ConsultaStatus.Concluida.ToString(), resultado.Status);
        _repositoryMock.Verify(r => r.Update(consulta), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void UpdateResultado_QuandoConsultaNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new ConsultaResultadoRequest
        {
            Diagnostico = "Animal saudavel",
            Observacoes = "Vacina aplicada",
            Status = ConsultaStatus.Concluida
        };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Consulta?)null);

        var resultado = _service.UpdateResultado(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Consulta>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoConsultaExiste_DeveRemoverERetornarTrue()
    {
        var consulta = new Consulta(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial");
        _repositoryMock.Setup(r => r.GetById(consulta.Id)).Returns(consulta);

        var resultado = _service.Delete(consulta.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(consulta), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoConsultaNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Consulta?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Consulta>()), Times.Never);
    }
    [Fact]
    public async Task GetPagedAsync_ComFiltrosValidos_DeveRetornarRespostaPaginada()
    {
        var consultas = new List<Consulta>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new TimeOnly(10, 0), "Vacina", "Presencial")
        };
        var parameters = new ConsultaQueryParameters { PageNumber = 1, PageSize = 10, Tipo = "Presencial" };
        _repositoryMock.Setup(r => r.GetPagedAsync(parameters, It.IsAny<CancellationToken>()))
            .ReturnsAsync((consultas, 1));

        var resultado = await _service.GetPagedAsync(parameters);

        Assert.NotNull(resultado);
        Assert.Single(resultado.Items);
        Assert.Equal(1, resultado.TotalCount);
    }
}
