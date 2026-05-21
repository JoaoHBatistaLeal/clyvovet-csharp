using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class SintomaService(ISintomaRepository sintomaRepository) : ISintomaService
{
    public IReadOnlyCollection<SintomaResponse> GetAll()
    {
        return sintomaRepository.GetAll().Select(x => new SintomaResponse(x)).ToList();
    }

    public SintomaResponse? GetById(Guid id)
    {
        var sintoma = sintomaRepository.GetById(id);
        return sintoma is null ? null : new SintomaResponse(sintoma);
    }

    public SintomaResponse Create(SintomaRequest request)
    {
        var sintoma = request.ToDomain();
        sintomaRepository.Add(sintoma);
        sintomaRepository.SaveChanges();
        return new SintomaResponse(sintoma);
    }

    public SintomaResponse? Update(Guid id, SintomaRequest request)
    {
        var sintoma = sintomaRepository.GetById(id);
        if (sintoma is null) return null;

        sintoma.Update(request.Nome);
        sintomaRepository.Update(sintoma);
        sintomaRepository.SaveChanges();
        return new SintomaResponse(sintoma);
    }

    public bool Delete(Guid id)
    {
        var sintoma = sintomaRepository.GetById(id);
        if (sintoma is null) return false;

        sintomaRepository.Delete(sintoma);
        sintomaRepository.SaveChanges();
        return true;
    }
}