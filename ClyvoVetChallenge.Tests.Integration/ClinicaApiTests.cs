using System.Net;
using System.Net.Http.Json;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class ClinicaApiTests
{
    private readonly HttpClient _client;

    public ClinicaApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task CriarClinica_ComDadosValidos_DeveRetornarStatus201Created()
    {
        var request = new ClinicaRequest
        {
            Cnpj = "11222333000144",
            Email = "clinica@sp.com",
            Telefone = "11999990000",
            Endereco = "Av Brasil, 1500",
            HorarioFuncionamento = "08:00 - 20:00"
        };

        var response = await _client.PostAsJsonAsync("/api/Clinica", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ClinicaResponse>();
        Assert.NotNull(body);
        Assert.Equal("11222333000144", body.Cnpj);
    }

    [Fact]
    public async Task ObterClinicaPorId_QuandoRegistroExiste_DeveRetornarStatus200Ok()
    {
        var request = new ClinicaRequest
        {
            Cnpj = "55666777000188",
            Email = "clinica2@sp.com",
            Telefone = "11988880000",
            Endereco = "Rua Oscar Freire, 200",
            HorarioFuncionamento = "24 Horas"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Clinica", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ClinicaResponse>();

        var response = await _client.GetAsync($"/api/Clinica/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ClinicaResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
    }

    [Fact]
    public async Task ObterClinicaPorId_QuandoRegistroNaoExiste_DeveRetornarStatus404NotFound()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/Clinica/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletarClinica_ComIdExistente_DeveRetornarStatus204NoContent()
    {
        var request = new ClinicaRequest
        {
            Cnpj = "99888777000122",
            Email = "clinica3@sp.com",
            Telefone = "11977770000",
            Endereco = "Rua Pamplona, 300",
            HorarioFuncionamento = "09:00 - 18:00"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Clinica", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ClinicaResponse>();

        var response = await _client.DeleteAsync($"/api/Clinica/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

