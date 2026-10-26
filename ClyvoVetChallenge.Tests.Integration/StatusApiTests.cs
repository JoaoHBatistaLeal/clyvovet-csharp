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

    [Fact]
    public async Task SwaggerJson_QuandoChamado_DeveRetornarEsquemaOpenApiValido()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("openapi", content);

        var docsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../docs"));
        if (Directory.Exists(docsDir))
        {
            await File.WriteAllTextAsync(Path.Combine(docsDir, "swagger.json"), content);
        }
    }
}
