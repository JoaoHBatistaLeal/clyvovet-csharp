using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IVeterinarioService
{
    IReadOnlyCollection<VeterinarioResponse> GetAll();
    VeterinarioResponse? GetById(Guid id);
    VeterinarioResponse Create(VeterinarioRequest request);
    VeterinarioResponse? Update(Guid id, VeterinarioRequest request);
    bool Delete(Guid id);
}