using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IClinicaService
{
    IReadOnlyCollection<ClinicaResponse> GetAll();
    ClinicaResponse? GetById(Guid id);
    ClinicaResponse Create(ClinicaRequest request);
    ClinicaResponse? Update(Guid id, ClinicaRequest request);
    bool Delete(Guid id);
}