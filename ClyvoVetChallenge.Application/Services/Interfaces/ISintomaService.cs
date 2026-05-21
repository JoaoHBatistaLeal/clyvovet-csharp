using System;
using System.Collections.Generic;
using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface ISintomaService
{
    IReadOnlyCollection<SintomaResponse> GetAll();
    SintomaResponse? GetById(Guid id);
    SintomaResponse Create(SintomaRequest request);
    SintomaResponse? Update(Guid id, SintomaRequest request);
    bool Delete(Guid id);
}