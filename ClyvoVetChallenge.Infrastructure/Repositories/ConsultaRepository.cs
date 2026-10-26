using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Enums;
using ClyvoVetChallenge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class ConsultaRepository(ClyvoVetContext context) : Repository<Consulta>(context), IConsultaRepository
{
    public IReadOnlyCollection<Consulta> GetByAnimalId(Guid animalId)
    {
        return _context.Set<Consulta>().Where(x => x.AnimalId == animalId).ToList();
    }

    public async Task<IReadOnlyCollection<Consulta>> ObterConsultasPorAnimalSqlAsync(Guid animalId, CancellationToken cancellationToken = default)
    {
        return await _context.Consultas
            .FromSqlRaw("SELECT * FROM \"TB_CONSULTA\" WHERE \"AnimalId\" = {0}", animalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CancelarConsultasAntigasAsync(int dias, CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsNpgsql())
        {
            return await _context.Database.ExecuteSqlRawAsync("CALL sp_cancelar_consultas_antigas({0})", [dias], cancellationToken);
        }

        return await _context.Database.ExecuteSqlRawAsync(
            "UPDATE \"TB_CONSULTA\" SET \"Status\" = 'Cancelada' WHERE \"Status\" = 'Agendada' AND \"DataConsulta\" < DATE('now', '-' || {0} || ' day')",
            dias);
    }

    public async Task<(IReadOnlyCollection<Consulta> Items, int TotalCount)> GetPagedAsync(ConsultaQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = Set.AsNoTracking().AsQueryable();

        if (parameters.AnimalId.HasValue && parameters.AnimalId.Value != Guid.Empty)
        {
            query = query.Where(x => x.AnimalId == parameters.AnimalId.Value);
        }

        if (parameters.VeterinarioId.HasValue && parameters.VeterinarioId.Value != Guid.Empty)
        {
            query = query.Where(x => x.VeterinarioId == parameters.VeterinarioId.Value);
        }

        if (parameters.ClinicaId.HasValue && parameters.ClinicaId.Value != Guid.Empty)
        {
            query = query.Where(x => x.ClinicaId == parameters.ClinicaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Status) && Enum.TryParse<ConsultaStatus>(parameters.Status, true, out var statusEnum))
        {
            query = query.Where(x => x.Status == statusEnum);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Tipo))
        {
            query = query.Where(x => x.Tipo.ToLower().Contains(parameters.Tipo.ToLower()));
        }

        if (parameters.DataInicio.HasValue)
        {
            query = query.Where(x => x.DataConsulta >= parameters.DataInicio.Value);
        }

        if (parameters.DataFim.HasValue)
        {
            query = query.Where(x => x.DataConsulta <= parameters.DataFim.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = parameters.SortBy?.ToLower() switch
        {
            "dataconsulta" => parameters.SortDescending ? query.OrderByDescending(x => x.DataConsulta).ThenByDescending(x => x.HoraConsulta) : query.OrderBy(x => x.DataConsulta).ThenBy(x => x.HoraConsulta),
            "status" => parameters.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "tipo" => parameters.SortDescending ? query.OrderByDescending(x => x.Tipo) : query.OrderBy(x => x.Tipo),
            _ => query.OrderByDescending(x => x.DataConsulta).ThenByDescending(x => x.HoraConsulta)
        };

        var skip = (parameters.PageNumber - 1) * parameters.PageSize;
        var items = await query.Skip(skip).Take(parameters.PageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
