using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Azure.Identity
{
    /// <summary>
    /// Provedor de credenciais padrao para autenticacao em recursos gerenciados do Azure (Managed Identity, Environment, etc.).
    /// </summary>
    public class DefaultAzureCredential
    {
        private static readonly HttpClient HttpClient = new();

        public virtual async Task<string?> GetTokenAsync(string resource = "https://vault.azure.net", CancellationToken cancellationToken = default)
        {
            // 1. Token direto via variavel de ambiente (se injetado em pipelines ou testes)
            var envToken = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_TOKEN")
                ?? Environment.GetEnvironmentVariable("AZURE_BEARER_TOKEN");
            if (!string.IsNullOrEmpty(envToken))
            {
                return envToken;
            }

            // 2. Managed Identity via IMDS / App Service / ACI (se configurado no ambiente Azure)
            var identityEndpoint = Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT");
            var identityHeader = Environment.GetEnvironmentVariable("IDENTITY_HEADER");
            if (!string.IsNullOrEmpty(identityEndpoint) && !string.IsNullOrEmpty(identityHeader))
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"{identityEndpoint}?resource={Uri.EscapeDataString(resource)}&api-version=2019-08-01");
                    request.Headers.Add("X-IDENTITY-HEADER", identityHeader);
                    var response = await HttpClient.SendAsync(request, cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                        if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                        {
                            return tokenProp.GetString();
                        }
                    }
                }
                catch
                {
                    // Managed identity nao disponivel neste runtime
                }
            }

            // 3. Azure IMDS padrao para Virtual Machines / Container Instances
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                var imdsRequest = new HttpRequestMessage(HttpMethod.Get, $"http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource={Uri.EscapeDataString(resource)}");
                imdsRequest.Headers.Add("Metadata", "true");
                var imdsResponse = await HttpClient.SendAsync(imdsRequest, linkedCts.Token);
                if (imdsResponse.IsSuccessStatusCode)
                {
                    using var stream = await imdsResponse.Content.ReadAsStreamAsync(linkedCts.Token);
                    using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: linkedCts.Token);
                    if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                    {
                        return tokenProp.GetString();
                    }
                }
            }
            catch
            {
                // IMDS nao acessivel localmente
            }

            return null;
        }
    }
}

namespace Azure.Extensions.AspNetCore.Configuration.Secrets
{
    /// <summary>
    /// Extensoes para registro do Azure Key Vault como fonte de configuracao nativa.
    /// </summary>
    public static class AzureKeyVaultConfigurationExtensions
    {
        public static IConfigurationBuilder AddAzureKeyVault(
            this IConfigurationBuilder builder,
            Uri vaultUri,
            global::Azure.Identity.DefaultAzureCredential? credential = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(vaultUri);

            return builder.Add(new AzureKeyVaultConfigurationSource(vaultUri, credential ?? new global::Azure.Identity.DefaultAzureCredential()));
        }
    }

    public class AzureKeyVaultConfigurationSource : IConfigurationSource
    {
        public Uri VaultUri { get; }
        public global::Azure.Identity.DefaultAzureCredential Credential { get; }

        public AzureKeyVaultConfigurationSource(Uri vaultUri, global::Azure.Identity.DefaultAzureCredential credential)
        {
            VaultUri = vaultUri;
            Credential = credential;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureKeyVaultConfigurationProvider(VaultUri, Credential);
        }
    }

    public class AzureKeyVaultConfigurationProvider : ConfigurationProvider
    {
        private readonly Uri _vaultUri;
        private readonly global::Azure.Identity.DefaultAzureCredential _credential;
        private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

        public AzureKeyVaultConfigurationProvider(Uri vaultUri, global::Azure.Identity.DefaultAzureCredential credential)
        {
            _vaultUri = vaultUri;
            _credential = credential;
        }

        public override void Load()
        {
            try
            {
                LoadAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Em caso de indisponibilidade de rede ou ausencia de credenciais locais, nao impede o startup
                // caso variaveis de ambiente ou configuracoes locais ja fornecam os valores necessarios.
                Console.WriteLine($"[AzureKeyVault] Aviso: Nao foi possivel carregar segredos diretamente do Azure Key Vault ({_vaultUri.Host}): {ex.Message}");
            }
        }

        private async Task LoadAsync()
        {
            var token = await _credential.GetTokenAsync("https://vault.azure.net");
            if (string.IsNullOrEmpty(token))
            {
                // Sem token de autenticacao Azure disponível
                return;
            }

            var baseUrl = _vaultUri.ToString().TrimEnd('/');
            var listUrl = $"{baseUrl}/secrets?api-version=7.4";

            var request = new HttpRequestMessage(HttpMethod.Get, listUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            if (!doc.RootElement.TryGetProperty("value", out var items) || items.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var item in items.EnumerateArray())
            {
                if (item.TryGetProperty("id", out var idProp))
                {
                    var secretUrl = idProp.GetString();
                    if (!string.IsNullOrEmpty(secretUrl))
                    {
                        await LoadSecretValueAsync(secretUrl, token);
                    }
                }
            }
        }

        private async Task LoadSecretValueAsync(string secretUrl, string token)
        {
            try
            {
                var url = secretUrl.Contains('?') ? $"{secretUrl}&api-version=7.4" : $"{secretUrl}?api-version=7.4";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await HttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                if (doc.RootElement.TryGetProperty("value", out var valProp))
                {
                    var secretVal = valProp.GetString();
                    // Extrai nome do secret a partir da URL: https://<vault>.vault.azure.net/secrets/<name>/<version>
                    var uri = new Uri(secretUrl);
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length >= 2 && segments[0] == "secrets")
                    {
                        var secretName = segments[1];
                        // Converte convencao Azure Key Vault 'Nome--SubNome' para 'Nome:SubNome'
                        var configKey = secretName.Replace("--", ":");
                        if (secretVal != null)
                        {
                            Data[configKey] = secretVal;
                        }
                    }
                }
            }
            catch
            {
                // Ignora falha de leitura de secret individual
            }
        }
    }
}
