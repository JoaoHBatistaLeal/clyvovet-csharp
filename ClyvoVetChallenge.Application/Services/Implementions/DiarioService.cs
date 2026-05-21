using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class DiarioService(IDiarioRepository diarioRepository, IDiarioSintomaRepository diarioSintomaRepository) : IDiarioService
{
    public IReadOnlyCollection<DiarioResponse> GetAll()
    {
        return diarioRepository.GetAll().Select(x => new DiarioResponse(x)).ToList();
    }

    public DiarioResponse? GetById(Guid id)
    {
        var diario = diarioRepository.GetById(id);
        return diario is null ? null : new DiarioResponse(diario);
    }

    public DiarioResponse Create(DiarioRequest request)
    {
        var diario = request.ToDomain();
        diarioRepository.Add(diario);
        diarioRepository.SaveChanges();

        if (request.Sintomas is not null && request.Sintomas.Any())
        {
            foreach (var sintoma in request.Sintomas)
            {
                var diarioSintoma = new DiarioSintoma(diario.Id, sintoma.SintomaId, sintoma.Intensidade);
                diarioSintomaRepository.Add(diarioSintoma);
            }
            diarioSintomaRepository.SaveChanges();
        }

        return new DiarioResponse(diario);
    }

    public DiarioResponse? Update(Guid id, DiarioRequest request)
    {
        var diario = diarioRepository.GetById(id);
        if (diario is null) return null;

        diario.Update(request.ObservacaoLivre);
        diarioRepository.Update(diario);
        diarioRepository.SaveChanges();
        return new DiarioResponse(diario);
    }

    public bool Delete(Guid id)
    {
        var diario = diarioRepository.GetById(id);
        if (diario is null) return false;

        diarioRepository.Delete(diario);
        diarioRepository.SaveChanges();
        return true;
    }
}