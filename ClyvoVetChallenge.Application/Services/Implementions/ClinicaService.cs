using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class ClinicaService(IClinicaRepository clinicaRepository) : IClinicaService
{
    public IReadOnlyCollection<ClinicaResponse> GetAll()
    {
        return clinicaRepository.GetAll().Select(x => new ClinicaResponse(x)).ToList();
    }

    public ClinicaResponse? GetById(Guid id)
    {
        var clinica = clinicaRepository.GetById(id);
        return clinica is null ? null : new ClinicaResponse(clinica);
    }

    public ClinicaResponse Create(ClinicaRequest request)
    {
        var clinica = request.ToDomain();
        clinicaRepository.Add(clinica);
        clinicaRepository.SaveChanges();
        return new ClinicaResponse(clinica);
    }

    public ClinicaResponse? Update(Guid id, ClinicaRequest request)
    {
        var clinica = clinicaRepository.GetById(id);
        if (clinica is null) return null;

        clinica.Update(request.Email, request.Telefone, request.HorarioFuncionamento);
        clinicaRepository.Update(clinica);
        clinicaRepository.SaveChanges();
        return new ClinicaResponse(clinica);
    }

    public bool Delete(Guid id)
    {
        var clinica = clinicaRepository.GetById(id);
        if (clinica is null) return false;

        clinicaRepository.Delete(clinica);
        clinicaRepository.SaveChanges();
        return true;
    }
}