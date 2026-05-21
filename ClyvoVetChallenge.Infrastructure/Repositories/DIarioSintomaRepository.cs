using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class DiarioSintomaRepository(ClyvoVetContext context) : Repository<DiarioSintoma>(context), IDiarioSintomaRepository
{
}