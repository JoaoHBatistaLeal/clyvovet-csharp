using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class PrescricaoService(IPrescricaoRepository prescricaoRepository) : IPrescricaoService
{
    public IReadOnlyCollection<PrescricaoResponse> GetAll()
    {
        return prescricaoRepository.GetAll().Select(x => new PrescricaoResponse(x)).ToList();
    }

    public PrescricaoResponse? GetById(Guid id)
    {
        var prescricao = prescricaoRepository.GetById(id);
        return prescricao is null ? null : new PrescricaoResponse(prescricao);
    }

    public PrescricaoResponse Create(PrescricaoRequest request)
    {
        var prescricao = request.ToDomain();
        prescricaoRepository.Add(prescricao);
        prescricaoRepository.SaveChanges();
        return new PrescricaoResponse(prescricao);
    }

    public PrescricaoResponse? Update(Guid id, PrescricaoRequest request)
    {
        var prescricao = prescricaoRepository.GetById(id);
        if (prescricao is null) return null;

        prescricao.Update(request.Dosagem, request.Frequencia, request.DuracaoDias, request.Observacao);
        prescricaoRepository.Update(prescricao);
        prescricaoRepository.SaveChanges();
        return new PrescricaoResponse(prescricao);
    }

    public bool Delete(Guid id)
    {
        var prescricao = prescricaoRepository.GetById(id);
        if (prescricao is null) return false;

        prescricaoRepository.Delete(prescricao);
        prescricaoRepository.SaveChanges();
        return true;
    }
}