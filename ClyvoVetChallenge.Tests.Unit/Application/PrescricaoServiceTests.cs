using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class PrescricaoServiceTests
{
    private readonly Mock<IPrescricaoRepository> _repositoryMock = new();
    private readonly PrescricaoService _service;

    public PrescricaoServiceTests()
    {
        _service = new PrescricaoService(_repositoryMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemPrescricoes_DeveRetornarColecao()
    {
        var prescricao = new Prescricao(Guid.NewGuid(), "Antibiotico", "50mg", "12/12h", 7, "Apos refeicao", DateTime.UtcNow);
        _repositoryMock.Setup(r => r.GetAll()).Returns([prescricao]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Antibiotico", resultado.First().Medicamento);
        _repositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarPrescricao()
    {
        var prescricao = new Prescricao(Guid.NewGuid(), "Antibiotico", "50mg", "12/12h", 7, "Apos refeicao", DateTime.UtcNow);
        _repositoryMock.Setup(r => r.GetById(prescricao.Id)).Returns(prescricao);

        var resultado = _service.GetById(prescricao.Id);

        Assert.NotNull(resultado);
        Assert.Equal(prescricao.Id, resultado.Id);
        _repositoryMock.Verify(r => r.GetById(prescricao.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Prescricao?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidos_DevePersistirERetornar()
    {
        var request = new PrescricaoRequest
        {
            ConsultaId = Guid.NewGuid(),
            Medicamento = "Anti-inflamatorio",
            Dosagem = "10mg",
            Frequencia = "24/24h",
            DuracaoDias = 5,
            Observacao = "Tomar pela manha",
            DataPrescricao = DateTime.UtcNow
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Anti-inflamatorio", resultado.Medicamento);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Prescricao>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var prescricao = new Prescricao(Guid.NewGuid(), "Antibiotico", "50mg", "12/12h", 7, "Apos refeicao", DateTime.UtcNow);
        var request = new PrescricaoRequest
        {
            Dosagem = "100mg",
            Frequencia = "8/8h",
            DuracaoDias = 10,
            Observacao = "Junto com alimento"
        };
        _repositoryMock.Setup(r => r.GetById(prescricao.Id)).Returns(prescricao);

        var resultado = _service.Update(prescricao.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("100mg", resultado.Dosagem);
        Assert.Equal("8/8h", resultado.Frequencia);
        _repositoryMock.Verify(r => r.Update(prescricao), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new PrescricaoRequest { Dosagem = "10mg", Frequencia = "24h", DuracaoDias = 5, Observacao = "" };
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Prescricao?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Prescricao>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var prescricao = new Prescricao(Guid.NewGuid(), "Antibiotico", "50mg", "12/12h", 7, "Apos refeicao", DateTime.UtcNow);
        _repositoryMock.Setup(r => r.GetById(prescricao.Id)).Returns(prescricao);

        var resultado = _service.Delete(prescricao.Id);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.Delete(prescricao), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetById(id)).Returns((Prescricao?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Prescricao>()), Times.Never);
    }
}
