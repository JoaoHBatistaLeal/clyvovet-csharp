using System;
using ClyvoVetChallenge.Application.DTOs.Common;
using ClyvoVetChallenge.Domain.Entities;

namespace ClyvoVetChallenge.Application.DTOs;

public class ConsultaResponse
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public Guid VeterinarioId { get; set; }
    public Guid ClinicaId { get; set; }
    public DateOnly DataConsulta { get; set; }
    public TimeOnly HoraConsulta { get; set; }
    public string Motivo { get; set; }
    public string? Diagnostico { get; set; }
    public string? Observacoes { get; set; }
    public string Status { get; set; }
    public string Tipo { get; set; }
    public List<LinkDto> Links { get; set; } = [];

    public ConsultaResponse() { }

    public ConsultaResponse(Consulta consulta)
    {
        Id = consulta.Id;
        AnimalId = consulta.AnimalId;
        VeterinarioId = consulta.VeterinarioId;
        ClinicaId = consulta.ClinicaId;
        DataConsulta = consulta.DataConsulta;
        HoraConsulta = consulta.HoraConsulta;
        Motivo = consulta.Motivo;
        Diagnostico = consulta.Diagnostico;
        Observacoes = consulta.Observacoes;
        Status = consulta.Status.ToString();
        Tipo = consulta.Tipo;
    }
}
