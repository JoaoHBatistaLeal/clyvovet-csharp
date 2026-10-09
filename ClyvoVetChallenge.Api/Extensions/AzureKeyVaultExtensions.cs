using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Azure.Identity
{
    public class DefaultAzureCredential
    {
        private static readonly HttpClient HttpClient = new();

        public virtual async Task<string?> GetTokenAsync(string resource = "https://vault.azure.net", CancellationToken cancellationToken = default)
        {
            var envToken = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_TOKEN")
                ?? Environment.GetEnvironmentVariable("AZURE_BEARER_TOKEN");
            if (!string.IsNullOrEmpty(envToken))
            {
                return envToken;
            }

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
                }
            }

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
            }

            return null;
        }
    }
}

namespace Azure.Extensions.AspNetCore.Configuration.Secrets
{
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
                Console.WriteLine($"[AzureKeyVault] Aviso: Nao foi possivel carregar segredos diretamente do Azure Key Vault ({_vaultUri.Host}): {ex.Message}");
            }
        }

        private async Task LoadAsync()
        {
            var token = await _credential.GetTokenAsync("https://vault.azure.net");
            if (string.IsNullOrEmpty(token))
            {
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
                    var uri = new Uri(secretUrl);
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length >= 2 && segments[0] == "secrets")
                    {
                        var secretName = segments[1];
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
            }
        }
    }
}
