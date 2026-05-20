
using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Historico : BaseEntity
{
    public Guid AnimalId { get; private set; }
    public Guid ConsultaId { get; private set; }
    public DateTime DataRegistro { get; private set; }
    public string Descricao { get; private set; }
    public string TipoEvento { get; private set; }
    public string Resultado { get; private set; }

    public Historico(Guid animalId, Guid consultaId, DateTime dataRegistro, string descricao, string tipoEvento, string resultado)
    {
        AnimalId = animalId;
        ConsultaId = consultaId;
        DataRegistro = dataRegistro;
        Descricao = descricao;
        TipoEvento = tipoEvento;
        Resultado = resultado;
    }

    public void Update(string descricao, string resultado)
    {
        Descricao = descricao;
        Resultado = resultado;
    }

    protected Historico() { }
}