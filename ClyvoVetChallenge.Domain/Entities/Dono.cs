using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Dono : BaseEntity
{
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    public string Endereco { get; private set; }

    public Dono(string nome, string cpf, string email, string telefone, DateOnly dataNascimento, string endereco)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        Endereco = endereco;
    }

    public void Update(string nome, string email, string telefone, string endereco)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        Endereco = endereco;
    }

    protected Dono() { }
}