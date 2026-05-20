using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Veterinario : BaseEntity
{
    public string Nome { get; private set; }
    public string Crmv { get; private set; }
    public string Especialidade { get; private set; }
    public string Telefone { get; private set; }
    public string Email { get; private set; }

    public Veterinario(string nome, string crmv, string especialidade, string telefone, string email)
    {
        Nome = nome;
        Crmv = crmv;
        Especialidade = especialidade;
        Telefone = telefone;
        Email = email;
    }

    public void UpdateContato(string telefone, string email)
    {
        Telefone = telefone;
        Email = email;
    }

    protected Veterinario() { }
}