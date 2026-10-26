using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IRecomendacaoIaService
{
    Task<RecomendacaoIaResponse> GerarRecomendacaoAsync(RecomendacaoIaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RecomendacaoIaResponse>> ObterHistoricoPorAnimalAsync(Guid animalId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RecomendacaoIaResponse>> ObterTodosHistoricosAsync(CancellationToken cancellationToken = default);
}
