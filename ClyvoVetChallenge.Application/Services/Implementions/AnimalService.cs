using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class AnimalService(IAnimalRepository animalRepository) : IAnimalService
{
    public IReadOnlyCollection<AnimalResponse> GetAll()
    {
        return animalRepository.GetAll().Select(x => new AnimalResponse(x)).ToList();
    }

    public AnimalResponse? GetById(Guid id)
    {
        var animal = animalRepository.GetById(id);
        return animal is null ? null : new AnimalResponse(animal);
    }

    public AnimalResponse Create(AnimalRequest request)
    {
        var animal = request.ToDomain();
        animalRepository.Add(animal);
        animalRepository.SaveChanges();
        return new AnimalResponse(animal);
    }

    public AnimalResponse? Update(Guid id, AnimalRequest request)
    {
        var animal = animalRepository.GetById(id);
        if (animal is null) return null;

        animal.Update(request.Nome, request.Peso);
        animalRepository.Update(animal);
        animalRepository.SaveChanges();
        return new AnimalResponse(animal);
    }

    public bool Delete(Guid id)
    {
        var animal = animalRepository.GetById(id);
        if (animal is null) return false;

        animalRepository.Delete(animal);
        animalRepository.SaveChanges();
        return true;
    }

    public async Task<PagedResponse<AnimalResponse>> GetPagedAsync(AnimalQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await animalRepository.GetPagedAsync(parameters, cancellationToken);
        var mappedItems = items.Select(x => new AnimalResponse(x)).ToList();
        return new PagedResponse<AnimalResponse>(mappedItems, parameters.PageNumber, parameters.PageSize, totalCount);
    }
}
