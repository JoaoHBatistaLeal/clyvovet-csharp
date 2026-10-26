using System.Net;
using System.Net.Http.Json;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Domain.Enums;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class ConsultaApiTests
{
    private readonly HttpClient _client;

    public ConsultaApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    private async Task<(Guid AnimalId, Guid VetId, Guid ClinicaId)> CriarDependenciasValidasAsync()
    {
        var donoRequest = new DonoRequest
        {
            Nome = "Dono Consulta",
            Cpf = $"{Random.Shared.NextInt64(10000000000, 99999999999)}",
            Email = $"dono{Guid.NewGuid():N}@consulta.com",
            Telefone = "11999998888",
            DataNascimento = new DateOnly(1985, 4, 12),
            Endereco = "Rua Central, 10"
        };
        var donoResp = await _client.PostAsJsonAsync("/api/Dono", donoRequest);
        var dono = await donoResp.Content.ReadFromJsonAsync<DonoResponse>();

        var animalRequest = new AnimalRequest
        {
            DonoId = dono!.Id,
            Nome = "Animal Consulta",
            Especie = "Canina",
            Raca = "Vira-Lata",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2022, 1, 1),
            Peso = 10m
        };
        var animalResp = await _client.PostAsJsonAsync("/api/Animal", animalRequest);
        var animal = await animalResp.Content.ReadFromJsonAsync<AnimalResponse>();

        var clinicaRequest = new ClinicaRequest
        {
            Cnpj = $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}",
            Email = $"clinica{Guid.NewGuid():N}@consulta.com",
            Telefone = "11988887777",
            Endereco = "Av Brasil, 100",
            HorarioFuncionamento = "24h"
        };
        var clinicaResp = await _client.PostAsJsonAsync("/api/Clinica", clinicaRequest);
        var clinica = await clinicaResp.Content.ReadFromJsonAsync<ClinicaResponse>();

        var vetRequest = new VeterinarioRequest
        {
            Nome = "Dr Consulta",
            Crmv = $"CRMV{Random.Shared.Next(10000, 99999)}",
            Especialidade = "Clinica Geral",
            Telefone = "11977776666",
            Email = $"vet{Guid.NewGuid():N}@consulta.com"
        };
        var vetResp = await _client.PostAsJsonAsync("/api/Veterinario", vetRequest);
        var vet = await vetResp.Content.ReadFromJsonAsync<VeterinarioResponse>();

        return (animal!.Id, vet!.Id, clinica!.Id);
    }

    [Fact]
    public async Task CriarConsulta_ComDadosValidos_DeveRetornarStatus201Created()
    {
        var (animalId, vetId, clinicaId) = await CriarDependenciasValidasAsync();
        var request = new ConsultaRequest
        {
            AnimalId = animalId,
            VeterinarioId = vetId,
            ClinicaId = clinicaId,
            DataConsulta = new DateOnly(2026, 8, 10),
            HoraConsulta = new TimeOnly(14, 0),
            Motivo = "Vacinacao anual",
            Tipo = "Presencial"
        };

        var response = await _client.PostAsJsonAsync("/api/Consulta", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConsultaResponse>();
        Assert.NotNull(body);
        Assert.Equal("Vacinacao anual", body.Motivo);
    }

    [Fact]
    public async Task ObterConsultaPorId_QuandoRegistroExiste_DeveRetornarStatus200Ok()
    {
        var (animalId, vetId, clinicaId) = await CriarDependenciasValidasAsync();
        var request = new ConsultaRequest
        {
            AnimalId = animalId,
            VeterinarioId = vetId,
            ClinicaId = clinicaId,
            DataConsulta = new DateOnly(2026, 8, 11),
            HoraConsulta = new TimeOnly(15, 0),
            Motivo = "Exame geral",
            Tipo = "Presencial"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Consulta", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ConsultaResponse>();

        var response = await _client.GetAsync($"/api/Consulta/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConsultaResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
    }

    [Fact]
    public async Task AtualizarResultadoConsulta_ComDadosValidos_DeveRetornarStatus200Ok()
    {
        var (animalId, vetId, clinicaId) = await CriarDependenciasValidasAsync();
        var request = new ConsultaRequest
        {
            AnimalId = animalId,
            VeterinarioId = vetId,
            ClinicaId = clinicaId,
            DataConsulta = new DateOnly(2026, 8, 12),
            HoraConsulta = new TimeOnly(16, 0),
            Motivo = "Retorno",
            Tipo = "Presencial"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Consulta", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ConsultaResponse>();

        var resultadoRequest = new ConsultaResultadoRequest
        {
            Diagnostico = "Recuperacao completa",
            Observacoes = "Alta medica liberada",
            Status = ConsultaStatus.Concluida
        };
        var response = await _client.PutAsJsonAsync($"/api/Consulta/{created!.Id}/resultado", resultadoRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConsultaResponse>();
        Assert.NotNull(body);
        Assert.Equal("Recuperacao completa", body.Diagnostico);
    }

    [Fact]
    public async Task DeletarConsulta_ComIdExistente_DeveRetornarStatus204NoContent()
    {
        var (animalId, vetId, clinicaId) = await CriarDependenciasValidasAsync();
        var request = new ConsultaRequest
        {
            AnimalId = animalId,
            VeterinarioId = vetId,
            ClinicaId = clinicaId,
            DataConsulta = new DateOnly(2026, 8, 13),
            HoraConsulta = new TimeOnly(17, 0),
            Motivo = "Cancelamento teste",
            Tipo = "Presencial"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Consulta", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ConsultaResponse>();

        var response = await _client.DeleteAsync($"/api/Consulta/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

