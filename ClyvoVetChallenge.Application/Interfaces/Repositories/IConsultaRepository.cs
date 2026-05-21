using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.Interfaces.Repositories;

public interface IConsultaRepository : IRepository<Consulta>
{
    IReadOnlyCollection<Consulta> GetByAnimalId(Guid animalId);
}