using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IPrescricaoService
{
    IReadOnlyCollection<PrescricaoResponse> GetAll();
    PrescricaoResponse? GetById(Guid id);
    PrescricaoResponse Create(PrescricaoRequest request);
    PrescricaoResponse? Update(Guid id, PrescricaoRequest request);
    bool Delete(Guid id);
}