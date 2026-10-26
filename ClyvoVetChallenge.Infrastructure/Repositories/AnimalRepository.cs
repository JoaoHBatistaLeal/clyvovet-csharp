using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class AnimalRepository(ClyvoVetContext context) : Repository<Animal>(context), IAnimalRepository
{
    public async Task<(IReadOnlyCollection<Animal> Items, int TotalCount)> GetPagedAsync(AnimalQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Set.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Nome))
        {
            query = query.Where(x => x.Nome.ToLower().Contains(parameters.Nome.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Especie))
        {
            query = query.Where(x => x.Especie.ToLower().Contains(parameters.Especie.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Raca))
        {
            query = query.Where(x => x.Raca.ToLower().Contains(parameters.Raca.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Sexo))
        {
            query = query.Where(x => x.Sexo.ToLower() == parameters.Sexo.ToLower());
        }

        if (parameters.DonoId.HasValue && parameters.DonoId.Value != Guid.Empty)
        {
            query = query.Where(x => x.DonoId == parameters.DonoId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = parameters.SortBy?.ToLower() switch
        {
            "nome" => parameters.SortDescending ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            "especie" => parameters.SortDescending ? query.OrderByDescending(x => x.Especie) : query.OrderBy(x => x.Especie),
            "peso" => parameters.SortDescending ? query.OrderByDescending(x => x.Peso) : query.OrderBy(x => x.Peso),
            "datanascimento" => parameters.SortDescending ? query.OrderByDescending(x => x.DataNascimento) : query.OrderBy(x => x.DataNascimento),
            _ => query.OrderBy(x => x.Nome)
        };

        var skip = (parameters.PageNumber - 1) * parameters.PageSize;
        var items = await query.Skip(skip).Take(parameters.PageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
