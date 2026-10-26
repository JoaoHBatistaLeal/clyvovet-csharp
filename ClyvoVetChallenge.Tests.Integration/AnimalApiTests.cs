using System.Net;
using System.Net.Http.Json;
using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class AnimalApiTests
{
    private readonly HttpClient _client;

    public AnimalApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateAuthenticatedClient();
    }

    private async Task<Guid> CriarDonoValidoAsync()
    {
        var donoRequest = new DonoRequest
        {
            Nome = "Dono Animal",
            Cpf = $"{Random.Shared.NextInt64(10000000000, 99999999999)}",
            Email = $"dono{Guid.NewGuid():N}@animal.com",
            Telefone = "11988887777",
            DataNascimento = new DateOnly(1992, 7, 20),
            Endereco = "Rua Teste, 123"
        };
        var donoResp = await _client.PostAsJsonAsync("/api/Dono", donoRequest);
        var dono = await donoResp.Content.ReadFromJsonAsync<DonoResponse>();
        return dono!.Id;
    }

    [Fact]
    public async Task CriarAnimal_ComDadosValidos_DeveRetornarStatus201Created()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Pipoca",
            Especie = "Felina",
            Raca = "Siames",
            Sexo = "Femea",
            DataNascimento = new DateOnly(2022, 3, 1),
            Peso = 4.2m
        };

        var response = await _client.PostAsJsonAsync("/api/Animal", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AnimalResponse>();
        Assert.NotNull(body);
        Assert.Equal("Pipoca", body.Nome);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    [Fact]
    public async Task ObterAnimalPorId_QuandoRegistroExiste_DeveRetornarStatus200Ok()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Bob",
            Especie = "Canina",
            Raca = "Beagle",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2021, 6, 15),
            Peso = 12.0m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Animal", request);
        var created = await createResponse.Content.ReadFromJsonAsync<AnimalResponse>();

        var response = await _client.GetAsync($"/api/Animal/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AnimalResponse>();
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
    }

    [Fact]
    public async Task ObterTodosAnimais_QuandoChamado_DeveRetornarStatus200Ok()
    {
        var response = await _client.GetAsync("/api/Animal");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<AnimalResponse>>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task AtualizarAnimal_ComDadosValidos_DeveRetornarStatus200Ok()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Luna",
            Especie = "Canina",
            Raca = "Poodle",
            Sexo = "Femea",
            DataNascimento = new DateOnly(2020, 1, 10),
            Peso = 5.0m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Animal", request);
        var created = await createResponse.Content.ReadFromJsonAsync<AnimalResponse>();

        var updateRequest = new AnimalRequest
        {
            DonoId = created!.DonoId,
            Nome = "Luna Silva",
            Especie = "Canina",
            Raca = "Poodle",
            Sexo = "Femea",
            DataNascimento = new DateOnly(2020, 1, 10),
            Peso = 5.5m
        };
        var response = await _client.PutAsJsonAsync($"/api/Animal/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AnimalResponse>();
        Assert.NotNull(body);
        Assert.Equal("Luna Silva", body.Nome);
        Assert.Equal(5.5m, body.Peso);
    }

    [Fact]
    public async Task DeletarAnimal_ComIdExistente_DeveRetornarStatus204NoContent()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Ted",
            Especie = "Canina",
            Raca = "Bulldog",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2019, 8, 20),
            Peso = 14.0m
        };
        var createResponse = await _client.PostAsJsonAsync("/api/Animal", request);
        var created = await createResponse.Content.ReadFromJsonAsync<AnimalResponse>();

        var response = await _client.DeleteAsync($"/api/Animal/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ObterAnimalPorId_QuandoRegistroNaoExiste_DeveRetornarStatus404NotFound()
    {
        var idInexistente = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/Animal/{idInexistente}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletarAnimal_ComIdInexistente_DeveRetornarStatus404NotFound()
    {
        var idInexistente = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/Animal/{idInexistente}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AtualizarAnimal_ComIdInexistente_DeveRetornarStatus404NotFound()
    {
        var idInexistente = Guid.NewGuid();
        var updateRequest = new AnimalRequest
        {
            DonoId = Guid.NewGuid(),
            Nome = "NaoExiste",
            Especie = "Canina",
            Raca = "ViraLata",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2020, 1, 1),
            Peso = 10m
        };

        var response = await _client.PutAsJsonAsync($"/api/Animal/{idInexistente}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CriarAnimal_ComPayloadInvalido_DeveRetornarStatus400BadRequest()
    {
        var requestInvalido = new { Nome = "" };

        var response = await _client.PostAsJsonAsync("/api/Animal", requestInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CriarAnimal_ComDataNascimentoFutura_DeveRetornarStatus422UnprocessableEntity()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Futuro",
            Especie = "Canina",
            Raca = "Beagle",
            Sexo = "Macho",
            DataNascimento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
            Peso = 10.0m
        };

        var response = await _client.PostAsJsonAsync("/api/Animal", request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    [Fact]
    public async Task ObterAnimaisPaginados_ComParametros_DeveRetornarStatus200OkELinksHateoas()
    {
        var donoId = await CriarDonoValidoAsync();
        var request = new AnimalRequest
        {
            DonoId = donoId,
            Nome = "Maximus",
            Especie = "Canina",
            Raca = "Pastor",
            Sexo = "Macho",
            DataNascimento = new DateOnly(2021, 1, 1),
            Peso = 30.0m
        };
        await _client.PostAsJsonAsync("/api/Animal", request);

        var response = await _client.GetAsync("/api/Animal/paged?pageNumber=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(json.TryGetProperty("items", out var itemsProp));
        Assert.True(json.TryGetProperty("links", out var linksProp));
        Assert.True(linksProp.GetArrayLength() > 0);
    }

    [Fact]
    public async Task ObterAnimais_SemAutenticacao_DeveRetornarStatus401Unauthorized()
    {
        using var factory = new ClyvoVetWebApplicationFactory();
        var unauthenticatedClient = factory.CreateClient();

        var response = await unauthenticatedClient.GetAsync("/api/Animal");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
