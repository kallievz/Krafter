using System.Text.Json;
using System.Text.Json.Serialization;
using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftExternalTokenProvider(
    ITenantSettingService tenantSettingService,
    IHttpClientFactory httpClientFactory) : IExternalTokenProvider, IScopedService
{
    public string ExternalClient => Features.Integrations.Common.ExternalClient.Munsoft;

    public string StorageKey => ExternalTokenStorageKeys.MunsoftToken;

    public TimeSpan TokenIdleTimeout => TimeSpan.FromSeconds(3600);

    public async Task<string> RequestTokenAsync(CancellationToken cancellationToken)
    {
        string baseAddress = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.MunsoftBaseAddess,
            string.Empty,
            cancellationToken).ConfigureAwait(false);

        string loginPath = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.MunsoftLogin,
            string.Empty,
            cancellationToken).ConfigureAwait(false);

        string userName = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.MunsoftUserName,
            string.Empty,
            cancellationToken).ConfigureAwait(false);

        string password = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.MunsoftPassword,
            string.Empty,
            cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(baseAddress))
        {
            throw new InvalidOperationException("Munsoft base address is not configured.");
        }

        if (string.IsNullOrWhiteSpace(loginPath))
        {
            throw new InvalidOperationException("Munsoft login path is not configured.");
        }

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Munsoft credentials are not configured.");
        }

        HttpClient httpClient = httpClientFactory.CreateClient(ExternalIntegrationHttpClientNames.MunsoftTokenClient);
        httpClient.BaseAddress = new Uri(baseAddress, UriKind.Absolute);

        var loginPayload = new MunsoftLoginRequest(userName, password);

        using HttpResponseMessage response = await httpClient
            .PostAsJsonAsync(loginPath, loginPayload, cancellationToken).ConfigureAwait(false);

        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            if ((int)response.StatusCode == 422 && IsMissingCredentialsValidationError(responseContent))
            {
                using HttpRequestMessage fallbackRequest = new(HttpMethod.Post, loginPath)
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["username"] = userName,
                        ["password"] = password
                    })
                };

                using HttpResponseMessage fallbackResponse = await httpClient
                    .SendAsync(fallbackRequest, cancellationToken).ConfigureAwait(false);

                string fallbackContent = await fallbackResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                if (!fallbackResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Munsoft token request failed with status {(int)fallbackResponse.StatusCode}: {fallbackContent}");
                }

                string? fallbackToken = ExtractTokenFromJson(fallbackContent);
                if (string.IsNullOrWhiteSpace(fallbackToken))
                {
                    throw new InvalidOperationException("Munsoft token response did not contain a token value.");
                }

                return fallbackToken;
            }

            throw new InvalidOperationException(
                $"Munsoft token request failed with status {(int)response.StatusCode}: {responseContent}");
        }

        string? token = ExtractTokenFromJson(responseContent);
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Munsoft token response did not contain a token value.");
        }

        return token;
    }

    private static string? ExtractTokenFromJson(string responseContent)
    {
        MunsoftTokenResponse? typedResponse = JsonSerializer.Deserialize<MunsoftTokenResponse>(responseContent);
        string? typedToken = typedResponse?.Profile?.Token;
        if (!string.IsNullOrWhiteSpace(typedToken))
        {
            return typedToken;
        }

        using JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
        return FindTokenValue(jsonDocument.RootElement);
    }

    private static string? FindTokenValue(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    string propertyName = property.Name;
                    if (string.Equals(propertyName, "token", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(propertyName, "access_token", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(propertyName, "accessToken", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(propertyName, "bearerToken", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(propertyName, "jwt", StringComparison.OrdinalIgnoreCase))
                    {
                        return property.Value.GetString();
                    }
                }

                string? nestedToken = FindTokenValue(property.Value);
                if (!string.IsNullOrWhiteSpace(nestedToken))
                {
                    return nestedToken;
                }
            }
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement childElement in element.EnumerateArray())
            {
                string? nestedToken = FindTokenValue(childElement);
                if (!string.IsNullOrWhiteSpace(nestedToken))
                {
                    return nestedToken;
                }
            }
        }

        return null;
    }

    private static bool IsMissingCredentialsValidationError(string responseContent)
    {
        try
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
            if (!jsonDocument.RootElement.TryGetProperty("errors", out JsonElement errorsElement))
            {
                return false;
            }

            bool missingUsername = errorsElement.TryGetProperty("username", out JsonElement usernameErrors)
                && usernameErrors.ToString().Contains("Field required", StringComparison.OrdinalIgnoreCase);
            bool missingPassword = errorsElement.TryGetProperty("password", out JsonElement passwordErrors)
                && passwordErrors.ToString().Contains("Field required", StringComparison.OrdinalIgnoreCase);

            return missingUsername && missingPassword;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private sealed record MunsoftLoginRequest(
        [property: JsonPropertyName("username")] string UserName,
        [property: JsonPropertyName("password")] string Password);

    private sealed record MunsoftTokenResponse(MunsoftProfile? Profile);

    private sealed record MunsoftProfile([property: JsonPropertyName("token")] string? Token);
}
