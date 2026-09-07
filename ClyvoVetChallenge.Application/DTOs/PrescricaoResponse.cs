using System;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class PrescricaoResponse
{
    public Guid Id { get; set; }
    public Guid ConsultaId { get; set; }
    public string Medicamento { get; set; }
    public string Dosagem { get; set; }
    public string Frequencia { get; set; }
    public int DuracaoDias { get; set; }
    public string Observacao { get; set; }
    public DateTime DataPrescricao { get; set; }

    public PrescricaoResponse() { }

    public PrescricaoResponse(Prescricao prescricao)
    {
        Id = prescricao.Id;
        ConsultaId = prescricao.ConsultaId;
        Medicamento = prescricao.Medicamento;
        Dosagem = prescricao.Dosagem;
        Frequencia = prescricao.Frequencia;
        DuracaoDias = prescricao.DuracaoDias;
        Observacao = prescricao.Observacao;
        DataPrescricao = prescricao.DataPrescricao;
    }
}