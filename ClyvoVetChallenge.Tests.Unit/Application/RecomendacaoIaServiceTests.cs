using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Exceptions;
using Moq;

namespace ClyvoVetChallenge.Tests.Unit.Application;

public class RecomendacaoIaServiceTests
{
    private readonly Mock<IRecomendacaoIaRepository> _repositoryMock = new();

    [Fact]
    public async Task GerarRecomendacaoAsync_ComSintomasCriticos_DeveRetornarNivelEmergencia()
    {
        var service = new RecomendacaoIaService(_repositoryMock.Object);
        var request = new RecomendacaoIaRequest
        {
            AnimalId = Guid.NewGuid(),
            NomeAnimal = "Thor",
            Especie = "Canina",
            IdadeAnos = 4,
            Sintomas = ["convulsao", "dificuldade respiratoria"]
        };

        var response = await service.GerarRecomendacaoAsync(request);

        Assert.NotNull(response);
        Assert.Equal("Emergencia", response.NivelUrgencia);
        Assert.NotEmpty(response.RecomendacoesCuidados);
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<HistoricoRecomendacaoIa>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GerarRecomendacaoAsync_ComSintomasAltos_DeveRetornarNivelAlta()
    {
        var service = new RecomendacaoIaService(_repositoryMock.Object);
        var request = new RecomendacaoIaRequest
        {
            AnimalId = Guid.NewGuid(),
            NomeAnimal = "Mia",
            Especie = "Felina",
            IdadeAnos = 2,
            Sintomas = ["febre", "vomito"]
        };

        var response = await service.GerarRecomendacaoAsync(request);

        Assert.NotNull(response);
        Assert.Equal("Alta", response.NivelUrgencia);
    }

    [Fact]
    public async Task GerarRecomendacaoAsync_SemAnimalId_DeveLancarBusinessRuleException()
    {
        var service = new RecomendacaoIaService(_repositoryMock.Object);
        var request = new RecomendacaoIaRequest
        {
            AnimalId = Guid.Empty,
            NomeAnimal = "Thor",
            Especie = "Canina",
            IdadeAnos = 4,
            Sintomas = ["febre"]
        };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.GerarRecomendacaoAsync(request));
    }

    [Fact]
    public async Task GerarRecomendacaoAsync_SemSintomas_DeveLancarBusinessRuleException()
    {
        var service = new RecomendacaoIaService(_repositoryMock.Object);
        var request = new RecomendacaoIaRequest
        {
            AnimalId = Guid.NewGuid(),
            NomeAnimal = "Thor",
            Especie = "Canina",
            IdadeAnos = 4,
            Sintomas = []
        };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.GerarRecomendacaoAsync(request));
    }

    [Fact]
    public async Task ObterHistoricoPorAnimalAsync_QuandoChamado_DeveRetornarListaDeRecomendacoes()
    {
        var animalId = Guid.NewGuid();
        var historicos = new List<HistoricoRecomendacaoIa>
        {
            new()
            {
                Id = "rec123",
                AnimalId = animalId,
                NomeAnimal = "Thor",
                Especie = "Canina",
                DiagnosticoSugerido = "Gastroenterite",
                NivelUrgencia = "Media"
            }
        };
        _repositoryMock.Setup(x => x.GetByAnimalIdAsync(animalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(historicos);
        var service = new RecomendacaoIaService(_repositoryMock.Object);

        var resultado = await service.ObterHistoricoPorAnimalAsync(animalId);

        Assert.Single(resultado);
        Assert.Equal("Thor", resultado.First().NomeAnimal);
    }
}
