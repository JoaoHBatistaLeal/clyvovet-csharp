using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
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
}
