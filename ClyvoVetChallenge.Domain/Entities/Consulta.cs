using System;
using ClyvoVetChallenge.Domain.Commons;
using ClyvoVetChallenge.Domain.Enums;

namespace ClyvoVetChallenge.Domain.Entities;

public class Consulta : BaseEntity
{
    public Guid AnimalId { get; private set; }
    public Guid VeterinarioId { get; private set; }
    public Guid ClinicaId { get; private set; }
    public DateOnly DataConsulta { get; private set; }
    public TimeOnly HoraConsulta { get; private set; }
    public string Motivo { get; private set; }
    public string Diagnostico { get; private set; }
    public string Observacoes { get; private set; }
    public ConsultaStatus Status { get; private set; }
    public string Tipo { get; private set; }

    public Animal Animal { get; private set; }
    public Veterinario Veterinario { get; private set; }
    public Clinica Clinica { get; private set; }

    public Consulta(Guid animalId, Guid veterinarioId, Guid clinicaId, DateOnly dataConsulta, TimeOnly horaConsulta, string motivo, string tipo)
    {
        AnimalId = animalId;
        VeterinarioId = veterinarioId;
        ClinicaId = clinicaId;
        DataConsulta = dataConsulta;
        HoraConsulta = horaConsulta;
        Motivo = motivo;
        Tipo = tipo;
        Status = ConsultaStatus.Agendada;
    }

    public void UpdateResultado(string diagnostico, string observacoes, ConsultaStatus status)
    {
        Diagnostico = diagnostico;
        Observacoes = observacoes;
        Status = status;
    }

    protected Consulta() { }
}