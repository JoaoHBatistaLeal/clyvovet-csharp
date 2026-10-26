using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.Interfaces.Repositories;

public interface IConsultaRepository : IRepository<Consulta>
{
    IReadOnlyCollection<Consulta> GetByAnimalId(Guid animalId);
    Task<IReadOnlyCollection<Consulta>> ObterConsultasPorAnimalSqlAsync(Guid animalId, CancellationToken cancellationToken = default);
    Task<int> CancelarConsultasAntigasAsync(int dias, CancellationToken cancellationToken = default);
}
