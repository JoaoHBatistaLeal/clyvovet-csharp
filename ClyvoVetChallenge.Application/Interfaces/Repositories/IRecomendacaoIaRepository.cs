using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.Interfaces.Repositories;

public interface IRecomendacaoIaRepository
{
    Task<IReadOnlyCollection<HistoricoRecomendacaoIa>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HistoricoRecomendacaoIa?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<HistoricoRecomendacaoIa>> GetByAnimalIdAsync(Guid animalId, CancellationToken cancellationToken = default);
    Task CreateAsync(HistoricoRecomendacaoIa entity, CancellationToken cancellationToken = default);
}
