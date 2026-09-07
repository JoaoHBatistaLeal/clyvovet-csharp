using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class DiarioServiceTests
{
    private readonly Mock<IDiarioRepository> _diarioRepoMock = new();
    private readonly Mock<IDiarioSintomaRepository> _sintomaRepoMock = new();
    private readonly DiarioService _service;

    public DiarioServiceTests()
    {
        _service = new DiarioService(_diarioRepoMock.Object, _sintomaRepoMock.Object);
    }

    [Fact]
    public void GetAll_QuandoExistemDiarios_DeveRetornarColecao()
    {
        var diario = new Diario(Guid.NewGuid(), new DateOnly(2026, 5, 1), new TimeOnly(12, 0), "Comportamento calmo");
        _diarioRepoMock.Setup(r => r.GetAll()).Returns([diario]);

        var resultado = _service.GetAll();

        Assert.Single(resultado);
        Assert.Equal("Comportamento calmo", resultado.First().ObservacaoLivre);
        _diarioRepoMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdExiste_DeveRetornarDiario()
    {
        var diario = new Diario(Guid.NewGuid(), new DateOnly(2026, 5, 1), new TimeOnly(12, 0), "Comportamento calmo");
        _diarioRepoMock.Setup(r => r.GetById(diario.Id)).Returns(diario);

        var resultado = _service.GetById(diario.Id);

        Assert.NotNull(resultado);
        Assert.Equal(diario.Id, resultado.Id);
        _diarioRepoMock.Verify(r => r.GetById(diario.Id), Times.Once);
    }

    [Fact]
    public void GetById_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        _diarioRepoMock.Setup(r => r.GetById(id)).Returns((Diario?)null);

        var resultado = _service.GetById(id);

        Assert.Null(resultado);
        _diarioRepoMock.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public void Create_QuandoDadosValidosSemSintomas_DevePersistirApenasDiario()
    {
        var request = new DiarioRequest
        {
            AnimalId = Guid.NewGuid(),
            DataRegistro = new DateOnly(2026, 5, 1),
            HoraRegistro = new TimeOnly(12, 0),
            ObservacaoLivre = "Comportamento calmo",
            Sintomas = []
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        Assert.Equal("Comportamento calmo", resultado.ObservacaoLivre);
        _diarioRepoMock.Verify(r => r.Add(It.IsAny<Diario>()), Times.Once);
        _diarioRepoMock.Verify(r => r.SaveChanges(), Times.Once);
        _sintomaRepoMock.Verify(r => r.Add(It.IsAny<DiarioSintoma>()), Times.Never);
    }

    [Fact]
    public void Create_QuandoDadosValidosComSintomas_DevePersistirDiarioESintomas()
    {
        var request = new DiarioRequest
        {
            AnimalId = Guid.NewGuid(),
            DataRegistro = new DateOnly(2026, 5, 1),
            HoraRegistro = new TimeOnly(12, 0),
            ObservacaoLivre = "Comportamento calmo",
            Sintomas = [new DiarioSintomaDto { SintomaId = Guid.NewGuid(), Intensidade = "Moderada" }]
        };

        var resultado = _service.Create(request);

        Assert.NotNull(resultado);
        _diarioRepoMock.Verify(r => r.Add(It.IsAny<Diario>()), Times.Once);
        _sintomaRepoMock.Verify(r => r.Add(It.IsAny<DiarioSintoma>()), Times.Once);
        _sintomaRepoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdExiste_DeveAtualizar()
    {
        var diario = new Diario(Guid.NewGuid(), new DateOnly(2026, 5, 1), new TimeOnly(12, 0), "Comportamento calmo");
        var request = new DiarioRequest { ObservacaoLivre = "Mais ativo agora" };
        _diarioRepoMock.Setup(r => r.GetById(diario.Id)).Returns(diario);

        var resultado = _service.Update(diario.Id, request);

        Assert.NotNull(resultado);
        Assert.Equal("Mais ativo agora", resultado.ObservacaoLivre);
        _diarioRepoMock.Verify(r => r.Update(diario), Times.Once);
        _diarioRepoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_QuandoIdNaoExiste_DeveRetornarNull()
    {
        var id = Guid.NewGuid();
        var request = new DiarioRequest { ObservacaoLivre = "Ativo" };
        _diarioRepoMock.Setup(r => r.GetById(id)).Returns((Diario?)null);

        var resultado = _service.Update(id, request);

        Assert.Null(resultado);
        _diarioRepoMock.Verify(r => r.Update(It.IsAny<Diario>()), Times.Never);
    }

    [Fact]
    public void Delete_QuandoIdExiste_DeveRemoverERetornarTrue()
    {
        var diario = new Diario(Guid.NewGuid(), new DateOnly(2026, 5, 1), new TimeOnly(12, 0), "Comportamento calmo");
        _diarioRepoMock.Setup(r => r.GetById(diario.Id)).Returns(diario);

        var resultado = _service.Delete(diario.Id);

        Assert.True(resultado);
        _diarioRepoMock.Verify(r => r.Delete(diario), Times.Once);
        _diarioRepoMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_QuandoIdNaoExiste_DeveRetornarFalse()
    {
        var id = Guid.NewGuid();
        _diarioRepoMock.Setup(r => r.GetById(id)).Returns((Diario?)null);

        var resultado = _service.Delete(id);

        Assert.False(resultado);
        _diarioRepoMock.Verify(r => r.Delete(It.IsAny<Diario>()), Times.Never);
    }
}
