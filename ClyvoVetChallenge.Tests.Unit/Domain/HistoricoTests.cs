using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class HistoricoTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var animalId = Guid.NewGuid();
        var consultaId = Guid.NewGuid();
        var data = new DateTime(2026, 3, 10, 8, 30, 0);

        var historico = new Historico(animalId, consultaId, data, "Vacinacao antirrabica", "Prevencao", "Aplicado sem reacoes");

        Assert.Equal(animalId, historico.AnimalId);
        Assert.Equal(consultaId, historico.ConsultaId);
        Assert.Equal(data, historico.DataRegistro);
        Assert.Equal("Vacinacao antirrabica", historico.Descricao);
        Assert.Equal("Prevencao", historico.TipoEvento);
        Assert.Equal("Aplicado sem reacoes", historico.Resultado);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarDescricaoEResultado()
    {
        var historico = new Historico(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Exame sangue", "Diagnostico", "Pendente");

        historico.Update("Exame sangue completo", "Normal");

        Assert.Equal("Exame sangue completo", historico.Descricao);
        Assert.Equal("Normal", historico.Resultado);
    }
}
