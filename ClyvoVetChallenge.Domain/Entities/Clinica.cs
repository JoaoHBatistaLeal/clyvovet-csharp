using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Clinica : BaseEntity
{
    public string Cnpj { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public string Endereco { get; private set; }
    public string HorarioFuncionamento { get; private set; }

    public Clinica(string cnpj, string email, string telefone, string endereco, string horarioFuncionamento)
    {
        Cnpj = cnpj;
        Email = email;
        Telefone = telefone;
        Endereco = endereco;
        HorarioFuncionamento = horarioFuncionamento;
    }

    public void Update(string email, string telefone, string horarioFuncionamento)
    {
        Email = email;
        Telefone = telefone;
        HorarioFuncionamento = horarioFuncionamento;
    }

    protected Clinica() { }
}