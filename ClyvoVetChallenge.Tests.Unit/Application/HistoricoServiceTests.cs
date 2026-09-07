using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class HistoricoServiceTests
{
    private readonly Mock<IHistoricoRepository> _repositoryMock = new();
    private readonly HistoricoService _service;

    public HistoricoServiceTests()
    {
        _service = new HistoricoService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemHistoricos_DeveRetornarColecao()
    {
        var historico = new Historico(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Cirurgia", "Procedimento", "Sucesso");
        _repositoryMock.Setup(r => r.GetAll()).Returns([historico]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Cirurgia", resultado.First().Descricao);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarHistorico()
    {
        var historico = new Historico(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Cirurgia", "Procedimento", "Sucesso");
        _repositoryMock.Setup(r => r.GetById(historico.Id)).Returns(historico);

        var resultado = _service.GetById(historico.Id);

        Assert.NotNull(resultado);
        Assert.Equal(historico.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(historico.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Historico?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new HistoricoRequest
        {
            AnimalId = Guid.NewGuid(),
            ConsultaId = Guid.NewGuid(),
            DataRegistro = DateTime.UtcNow,
            Descricao = "Exame de sangue",
            TipoEvento = "Exame",
            Resultado = "Normal"
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Exame de sangue", resultado.Descricao);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Historico>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var historico = new Historico(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Cirurgia", "Procedimento", "Sucesso");
        var request = new HistoricoRequest
        {
            Descricao = "Cirurgia com retorno",
            Resultado = "Excelente recuperacao"
        };
        _repositoryMock.Setup(r => r.GetById(historico.Id)).Returns(historico);

        var resultado = _service.Update(historico.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Cirurgia com retorno", resultado.Descricao);
        _repositoryMock.Verify(r => r.Update(historico), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new HistoricoRequest { Descricao = "Cirurgia", Resultado = "Normal" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Historico?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Historico>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var historico = new Historico(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Cirurgia", "Procedimento", "Sucesso");
        _repositoryMock.Setup(r => r.GetById(historico.Id)).Returns(historico);

        var resultado = _service.Delete(historico.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(historico), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Historico?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Historico>()), Times.Never);
    }
}
