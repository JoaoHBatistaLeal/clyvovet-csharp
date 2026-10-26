using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IConsultaService
{
    IReadOnlyCollection<ConsultaResponse> GetAll();
    ConsultaResponse? GetById(Guid id);
    IReadOnlyCollection<ConsultaResponse> GetByAnimalId(Guid animalId);
    ConsultaResponse Create(ConsultaRequest request);
    ConsultaResponse? UpdateResultado(Guid id, ConsultaResultadoRequest request);
    bool Delete(Guid id);
    Task<PagedResponse<ConsultaResponse>> GetPagedAsync(ConsultaQueryParameters parameters, CancellationToken cancellationToken = default);
}
