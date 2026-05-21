using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class ConsultaRepository(ClyvoVetContext context) : Repository<Consulta>(context), IConsultaRepository
{
    public IReadOnlyCollection<Consulta> GetByAnimalId(Guid animalId)
    {
        return _context.Set<Consulta>().Where(x => x.AnimalId == animalId).ToList();
    }
}