using System;
using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Animal : BaseEntity
{
    public Guid DonoId { get; private set; }
    public string Nome { get; private set; }
    public string Especie { get; private set; }
    public string Raca { get; private set; }
    public string Sexo { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    public decimal Peso { get; private set; }

    public Dono Dono { get; private set; }

    public Animal(Guid donoId, string nome, string especie, string raca, string sexo, DateOnly dataNascimento, decimal peso)
    {
        DonoId = donoId;
        Nome = nome;
        Especie = especie;
        Raca = raca;
        Sexo = sexo;
        DataNascimento = dataNascimento;
        Peso = peso;
    }

    public void Update(string nome, decimal peso)
    {
        Nome = nome;
        Peso = peso;
    }

    protected Animal() { }
}