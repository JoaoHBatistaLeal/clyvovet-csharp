using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class VeterinarioRepository(ClyvoVetContext context) : Repository<Veterinario>(context), IVeterinarioRepository
{
}