using System;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class AnimalResponse
{
    public Guid Id { get; set; }
    public Guid DonoId { get; set; }
    public string Nome { get; set; }
    public string Especie { get; set; }
    public string Raca { get; set; }
    public string Sexo { get; set; }
    public DateOnly DataNascimento { get; set; }
    public decimal Peso { get; set; }
    public List<LinkDto> Links { get; set; } = [];

    public AnimalResponse() { }

    public AnimalResponse(Animal animal)
    {
        Id = animal.Id;
        DonoId = animal.DonoId;
        Nome = animal.Nome;
        Especie = animal.Especie;
        Raca = animal.Raca;
        Sexo = animal.Sexo;
        DataNascimento = animal.DataNascimento;
        Peso = animal.Peso;
    }
}
