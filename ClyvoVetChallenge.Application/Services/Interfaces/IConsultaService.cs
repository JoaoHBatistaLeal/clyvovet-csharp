using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IConsultaService
{
    IReadOnlyCollection<ConsultaResponse> GetAll();
    ConsultaResponse? GetById(Guid id);
    IReadOnlyCollection<ConsultaResponse> GetByAnimalId(Guid animalId);
    ConsultaResponse Create(ConsultaRequest request);
    ConsultaResponse? UpdateResultado(Guid id, ConsultaResultadoRequest request);
    bool Delete(Guid id);
}