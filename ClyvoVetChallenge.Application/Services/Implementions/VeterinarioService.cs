using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class VeterinarioService(IVeterinarioRepository veterinarioRepository) : IVeterinarioService
{
    public IReadOnlyCollection<VeterinarioResponse> GetAll()
    {
        return veterinarioRepository.GetAll().Select(x => new VeterinarioResponse(x)).ToList();
    }

    public VeterinarioResponse? GetById(Guid id)
    {
        var veterinario = veterinarioRepository.GetById(id);
        return veterinario is null ? null : new VeterinarioResponse(veterinario);
    }

    public VeterinarioResponse Create(VeterinarioRequest request)
    {
        var veterinario = request.ToDomain();
        veterinarioRepository.Add(veterinario);
        veterinarioRepository.SaveChanges();
        return new VeterinarioResponse(veterinario);
    }

    public VeterinarioResponse? Update(Guid id, VeterinarioRequest request)
    {
        var veterinario = veterinarioRepository.GetById(id);
        if (veterinario is null) return null;

        veterinario.UpdateContato(request.Telefone, request.Email);
        veterinarioRepository.Update(veterinario);
        veterinarioRepository.SaveChanges();
        return new VeterinarioResponse(veterinario);
    }

    public bool Delete(Guid id)
    {
        var veterinario = veterinarioRepository.GetById(id);
        if (veterinario is null) return false;

        veterinarioRepository.Delete(veterinario);
        veterinarioRepository.SaveChanges();
        return true;
    }
}