using System;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class HistoricoResponse
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public Guid ConsultaId { get; set; }
    public DateTime DataRegistro { get; set; }
    public string Descricao { get; set; }
    public string TipoEvento { get; set; }
    public string Resultado { get; set; }

    public HistoricoResponse() { }

    public HistoricoResponse(Historico historico)
    {
        Id = historico.Id;
        AnimalId = historico.AnimalId;
        ConsultaId = historico.ConsultaId;
        DataRegistro = historico.DataRegistro;
        Descricao = historico.Descricao;
        TipoEvento = historico.TipoEvento;
        Resultado = historico.Resultado;
    }
}