using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class HistoricoService(IHistoricoRepository historicoRepository) : IHistoricoService
{
    public IReadOnlyCollection<HistoricoResponse> GetAll()
    {
        return historicoRepository.GetAll().Select(x => new HistoricoResponse(x)).ToList();
    }

    public HistoricoResponse? GetById(Guid id)
    {
        var historico = historicoRepository.GetById(id);
        return historico is null ? null : new HistoricoResponse(historico);
    }

    public HistoricoResponse Create(HistoricoRequest request)
    {
        var historico = request.ToDomain();
        historicoRepository.Add(historico);
        historicoRepository.SaveChanges();
        return new HistoricoResponse(historico);
    }

    public HistoricoResponse? Update(Guid id, HistoricoRequest request)
    {
        var historico = historicoRepository.GetById(id);
        if (historico is null) return null;

        historico.Update(request.Descricao, request.Resultado);
        historicoRepository.Update(historico);
        historicoRepository.SaveChanges();
        return new HistoricoResponse(historico);
    }

    public bool Delete(Guid id)
    {
        var historico = historicoRepository.GetById(id);
        if (historico is null) return false;

        historicoRepository.Delete(historico);
        historicoRepository.SaveChanges();
        return true;
    }
}