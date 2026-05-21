using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IDonoService
{
    IReadOnlyCollection<DonoResponse> GetAll();
    DonoResponse? GetById(Guid id);
    DonoResponse Create(DonoRequest request);
    DonoResponse? Update(Guid id, DonoRequest request);
    bool Delete(Guid id);
}