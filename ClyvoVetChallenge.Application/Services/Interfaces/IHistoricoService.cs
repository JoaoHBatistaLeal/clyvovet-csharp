using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IHistoricoService
{
    IReadOnlyCollection<HistoricoResponse> GetAll();
    HistoricoResponse? GetById(Guid id);
    HistoricoResponse Create(HistoricoRequest request);
    HistoricoResponse? Update(Guid id, HistoricoRequest request);
    bool Delete(Guid id);
}