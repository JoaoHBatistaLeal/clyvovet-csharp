using System.Net;
using System.Text.Json;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class MonitoringApiTests
{
    private readonly HttpClient _client;

    public MonitoringApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_QuandoChamado_DeveRetornarFormatoJsonComChecksEDuracao()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("status", out var statusProp));
        Assert.True(root.TryGetProperty("checks", out var checksProp));
        Assert.True(root.TryGetProperty("duration", out _));
        Assert.False(string.IsNullOrWhiteSpace(statusProp.GetString()));
        Assert.Equal(JsonValueKind.Array, checksProp.ValueKind);
    }

    [Fact]
    public async Task GetLive_QuandoChamado_DeveRetornarStatus200Ok()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMetrics_QuandoChamado_DeveRetornarMetricasPrometheus()
    {
        var response = await _client.GetAsync("/metrics");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.Contains("# TYPE", content);
    }

    [Fact]
    public async Task RequisicaoHttp_QuandoProcessada_DeveRetornarHeaderCorrelationId()
    {
        var response = await _client.GetAsync("/api/Animal");

        Assert.True(response.Headers.Contains("X-Correlation-Id"));
        var correlationId = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();
        Assert.False(string.IsNullOrWhiteSpace(correlationId));
    }
}
