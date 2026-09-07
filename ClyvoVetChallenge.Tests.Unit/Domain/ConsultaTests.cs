using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Enums;

namespace ClyvoVetChallenge.Tests.Unit.Domain;

public class ConsultaTests
{
    [Fact]
    public void Construtor_ComDadosValidos_DeveInstanciarEDefinirStatusComoAgendada()
    {
        var animalId = Guid.NewGuid();
        var vetId = Guid.NewGuid();
        var clinicaId = Guid.NewGuid();
        var data = new DateOnly(2026, 6, 15);
        var hora = new TimeOnly(14, 30);

        var consulta = new Consulta(animalId, vetId, clinicaId, data, hora, "Rotina", "Presencial");

        Assert.Equal(animalId, consulta.AnimalId);
        Assert.Equal(vetId, consulta.VeterinarioId);
        Assert.Equal(clinicaId, consulta.ClinicaId);
        Assert.Equal(data, consulta.DataConsulta);
        Assert.Equal(hora, consulta.HoraConsulta);
        Assert.Equal("Rotina", consulta.Motivo);
        Assert.Equal("Presencial", consulta.Tipo);
        Assert.Equal(ConsultaStatus.Agendada, consulta.Status);
    }

    [Fact]
    public void UpdateResultado_ComNovosDados_DeveAtualizarDiagnosticoObservacoesEStatus()
    {
        var consulta = new Consulta(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 15), new TimeOnly(14, 30), "Rotina", "Presencial");

        consulta.UpdateResultado("Otite canina", "Aplicar gotas diariamente", ConsultaStatus.Concluida);

        Assert.Equal("Otite canina", consulta.Diagnostico);
        Assert.Equal("Aplicar gotas diariamente", consulta.Observacoes);
        Assert.Equal(ConsultaStatus.Concluida, consulta.Status);
    }
}
