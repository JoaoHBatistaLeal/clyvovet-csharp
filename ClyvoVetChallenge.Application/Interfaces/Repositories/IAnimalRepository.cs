using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.Interfaces.Repositories;

public interface IAnimalRepository : IRepository<Animal>
{
    Task<(IReadOnlyCollection<Animal> Items, int TotalCount)> GetPagedAsync(AnimalQueryParameters parameters, CancellationToken cancellationToken = default);
}
