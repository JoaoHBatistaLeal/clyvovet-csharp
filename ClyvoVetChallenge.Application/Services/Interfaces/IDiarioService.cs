using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IDiarioService
{
    IReadOnlyCollection<DiarioResponse> GetAll();
    DiarioResponse? GetById(Guid id);
    DiarioResponse Create(DiarioRequest request);
    DiarioResponse? Update(Guid id, DiarioRequest request);
    bool Delete(Guid id);
}