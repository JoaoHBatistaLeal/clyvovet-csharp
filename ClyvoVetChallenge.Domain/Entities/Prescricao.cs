using System;
using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Domain.Entities;

public class Prescricao : BaseEntity
{
    public Guid ConsultaId { get; private set; }
    public string Medicamento { get; private set; }
    public string Dosagem { get; private set; }
    public string Frequencia { get; private set; }
    public int DuracaoDias { get; private set; }
    public string Observacao { get; private set; }
    public DateTime DataPrescricao { get; private set; }

    public Prescricao(Guid consultaId, string medicamento, string dosagem, string frequencia, int duracaoDias, string observacao, DateTime dataPrescricao)
    {
        ConsultaId = consultaId;
        Medicamento = medicamento;
        Dosagem = dosagem;
        Frequencia = frequencia;
        DuracaoDias = duracaoDias;
        Observacao = observacao;
        DataPrescricao = dataPrescricao;
    }

    public void Update(string dosagem, string frequencia, int duracaoDias, string observacao)
    {
        Dosagem = dosagem;
        Frequencia = frequencia;
        DuracaoDias = duracaoDias;
        Observacao = observacao;
    }

    protected Prescricao() { }
}