using ClyvoVetChallenge.Application.DTOs;

namespace ClyvoVetChallenge.Application.Services.Interfaces;

public interface IAnimalService
{
    IReadOnlyCollection<AnimalResponse> GetAll();
    AnimalResponse? GetById(Guid id);
    AnimalResponse Create(AnimalRequest request);
    AnimalResponse? Update(Guid id, AnimalRequest request);
    bool Delete(Guid id);
}