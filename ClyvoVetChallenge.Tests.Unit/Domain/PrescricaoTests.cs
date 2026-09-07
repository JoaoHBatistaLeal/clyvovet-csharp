using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class PrescricaoTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarPropriedadesCorretamente()
    {
        var consultaId = Guid.NewGuid();
        var data = new DateTime(2026, 4, 1, 10, 0, 0);

        var prescricao = new Prescricao(consultaId, "Antibiotico", "50mg", "A cada 12h", 7, "Apos refeicoes", data);

        Assert.Equal(consultaId, prescricao.ConsultaId);
        Assert.Equal("Antibiotico", prescricao.Medicamento);
        Assert.Equal("50mg", prescricao.Dosagem);
        Assert.Equal("A cada 12h", prescricao.Frequencia);
        Assert.Equal(7, prescricao.DuracaoDias);
        Assert.Equal("Apos refeicoes", prescricao.Observacao);
        Assert.Equal(data, prescricao.DataPrescricao);
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarCamposDaPrescricao()
    {
        var prescricao = new Prescricao(Guid.NewGuid(), "Antibiotico", "50mg", "A cada 12h", 7, "Apos refeicoes", DateTime.UtcNow);

        prescricao.Update("100mg", "A cada 8h", 10, "Em jejum");

        Assert.Equal("100mg", prescricao.Dosagem);
        Assert.Equal("A cada 8h", prescricao.Frequencia);
        Assert.Equal(10, prescricao.DuracaoDias);
        Assert.Equal("Em jejum", prescricao.Observacao);
    }
}
