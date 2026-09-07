using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class DonoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public DateOnly DataNascimento { get; set; }
    public string Endereco { get; set; }

    public DonoResponse() { }

    public DonoResponse(Dono dono)
    {
        Id = dono.Id;
        Nome = dono.Nome;
        Cpf = dono.Cpf;
        Email = dono.Email;
        Telefone = dono.Telefone;
        DataNascimento = dono.DataNascimento;
        Endereco = dono.Endereco;
    }
}