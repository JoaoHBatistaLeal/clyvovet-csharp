using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class StatusApiTests
{
    private readonly HttpClient _client;

    public StatusApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ObterStatus_SemAutenticacao_DeveRetornarStatus200OkEInformacoesDeDeploy()
    {
        var response = await _client.GetAsync("/api/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("application", out var appProp));
        Assert.Equal("Clyvo Vet API", appProp.GetString());
        Assert.True(json.TryGetProperty("version", out var verProp));
        Assert.Equal("1.0.0-sprint4", verProp.GetString());
        Assert.True(json.TryGetProperty("status", out var statusProp));
        Assert.Equal("Online", statusProp.GetString());
    }
}
