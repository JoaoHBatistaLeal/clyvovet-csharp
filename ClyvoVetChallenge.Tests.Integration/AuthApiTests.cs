using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoVetChallenge.Api.Controllers;
using ClyvoVetChallenge.Tests.Integration.Fixtures;

namespace ClyvoVetChallenge.Tests.Integration;

[Collection("IntegrationTests")]
public class AuthApiTests
{
    private readonly HttpClient _client;

    public AuthApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AcessarPerfil_SemAutenticacao_DeveRetornarStatus401Unauthorized()
    {
        var response = await _client.GetAsync("/api/auth/perfil");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AcessarPerfil_ComTokenInvalido_DeveRetornarStatus401Unauthorized()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/auth/perfil");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "token_invalido_12345");

        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_DeveRetornarStatus401Unauthorized()
    {
        var loginRequest = new LoginRequest("usuario_errado", "senha_errada");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComPayloadInvalido_DeveRetornarStatus400BadRequest()
    {
        var loginRequest = new LoginRequest("", "");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarStatus200OkEToken()
    {
        var loginRequest = new LoginRequest("admin", "admin123");

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out var tokenProp));
        Assert.False(string.IsNullOrWhiteSpace(tokenProp.GetString()));
    }

    [Fact]
    public async Task AcessarPerfil_ComTokenValido_DeveRetornarStatus200Ok()
    {
        var loginRequest = new LoginRequest("admin", "admin123");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var json = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/auth/perfil");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
