using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class DonoService(IDonoRepository donoRepository) : IDonoService
{
    public IReadOnlyCollection<DonoResponse> GetAll()
    {
        return donoRepository.GetAll().Select(x => new DonoResponse(x)).ToList();
    }

    public DonoResponse? GetById(Guid id)
    {
        var dono = donoRepository.GetById(id);
        return dono is null ? null : new DonoResponse(dono);
    }

    public DonoResponse Create(DonoRequest request)
    {
        var dono = request.ToDomain();
        donoRepository.Add(dono);
        donoRepository.SaveChanges();
        return new DonoResponse(dono);
    }

    public DonoResponse? Update(Guid id, DonoRequest request)
    {
        var dono = donoRepository.GetById(id);
        if (dono is null) return null;

        dono.Update(request.Nome, request.Email, request.Telefone, request.Endereco);
        donoRepository.Update(dono);
        donoRepository.SaveChanges();
        return new DonoResponse(dono);
    }

    public bool Delete(Guid id)
    {
        var dono = donoRepository.GetById(id);
        if (dono is null) return false;

        donoRepository.Delete(dono);
        donoRepository.SaveChanges();
        return true;
    }
}