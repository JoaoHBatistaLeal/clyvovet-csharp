using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class ConsultaService(IConsultaRepository consultaRepository) : IConsultaService
{
    public IReadOnlyCollection<ConsultaResponse> GetAll()
    {
        return consultaRepository.GetAll().Select(x => new ConsultaResponse(x)).ToList();
    }

    public ConsultaResponse? GetById(Guid id)
    {
        var consulta = consultaRepository.GetById(id);
        return consulta is null ? null : new ConsultaResponse(consulta);
    }

    public IReadOnlyCollection<ConsultaResponse> GetByAnimalId(Guid animalId)
    {
        return consultaRepository.GetByAnimalId(animalId).Select(x => new ConsultaResponse(x)).ToList();
    }

    public ConsultaResponse Create(ConsultaRequest request)
    {
        var consulta = request.ToDomain();
        consultaRepository.Add(consulta);
        consultaRepository.SaveChanges();
        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse? UpdateResultado(Guid id, ConsultaResultadoRequest request)
    {
        var consulta = consultaRepository.GetById(id);
        if (consulta is null) return null;

        consulta.UpdateResultado(request.Diagnostico, request.Observacoes, request.Status);
        consultaRepository.Update(consulta);
        consultaRepository.SaveChanges();
        return new ConsultaResponse(consulta);
    }

    public bool Delete(Guid id)
    {
        var consulta = consultaRepository.GetById(id);
        if (consulta is null) return false;

        consultaRepository.Delete(consulta);
        consultaRepository.SaveChanges();
        return true;
    }

    public async Task<PagedResponse<ConsultaResponse>> GetPagedAsync(ConsultaQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await consultaRepository.GetPagedAsync(parameters, cancellationToken);
        var mappedItems = items.Select(x => new ConsultaResponse(x)).ToList();
        return new PagedResponse<ConsultaResponse>(mappedItems, parameters.PageNumber, parameters.PageSize, totalCount);
    }
}
