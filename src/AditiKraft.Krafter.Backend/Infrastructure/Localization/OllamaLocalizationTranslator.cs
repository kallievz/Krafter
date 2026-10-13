using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Localization;

public interface IOllamaLocalizationTranslator
{
    public Task<bool> IsModelServedAsync(
        string ollamaEndpoint,
        string model,
        CancellationToken cancellationToken);

    public Task<string?> TranslateAsync(
        string sourceText,
        string sourceLanguageCode,
        string sourceLanguageName,
        string targetLanguageCode,
        string targetLanguageName,
        string ollamaEndpoint,
        string model,
        CancellationToken cancellationToken);
}

internal sealed class OllamaLocalizationTranslator(IOptions<LocalizationOllamaOptions> localizationOptions) : IOllamaLocalizationTranslator
{

    public async Task<bool> IsModelServedAsync(
        string ollamaEndpoint,
        string model,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ollamaEndpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        using HttpClient client = CreateClient(localizationOptions.Value);
        string showRequestUri = BuildShowRequestUri(ollamaEndpoint);

        using HttpResponseMessage showResponse = await client.PostAsJsonAsync(
            showRequestUri,
            new
            {
                model
            },
            cancellationToken).ConfigureAwait(false);

        if (showResponse.IsSuccessStatusCode)
        {
            return true;
        }

        string pullRequestUri = BuildPullRequestUri(ollamaEndpoint);
        using HttpResponseMessage pullResponse = await client.PostAsJsonAsync(
            pullRequestUri,
            new
            {
                model,
                stream = false
            },
            cancellationToken).ConfigureAwait(false);

        if (!pullResponse.IsSuccessStatusCode)
        {
            return false;
        }

        using HttpResponseMessage showResponseAfterPull = await client.PostAsJsonAsync(
            showRequestUri,
            new
            {
                model
            },
            cancellationToken).ConfigureAwait(false);

        return showResponseAfterPull.IsSuccessStatusCode;
    }

    public async Task<string?> TranslateAsync(
        string sourceText,
        string sourceLanguageCode,
        string sourceLanguageName,
        string targetLanguageCode,
        string targetLanguageName,
        string ollamaEndpoint,
        string model,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceText);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLanguageCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetLanguageCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(ollamaEndpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        string normalizedSourceLanguageName = ResolveLanguageName(sourceLanguageCode, sourceLanguageName);
        string normalizedTargetLanguageName = ResolveLanguageName(targetLanguageCode, targetLanguageName);

        string prompt =
            "You are translating a UI localization value.\n" +
            $"Source language: {normalizedSourceLanguageName} ({sourceLanguageCode}).\n" +
            $"Target language: {normalizedTargetLanguageName} ({targetLanguageCode}).\n" +
            "Provide a natural target-language translation with clear phonetic readability for end users.\n" +
            "Preserve placeholders/tokens exactly (for example: {0}, {name}, %s, {{value}}, HTML tags, punctuation, and spacing patterns where applicable).\n" +
            "Translate only the source value.\n" +
            "Output rules (mandatory):\n" +
            "- Output only the translation text.\n" +
            "- Do not include explanations, labels, quotes, markdown, or prefix/suffix text.\n" +
            "- Do not wrap output in code fences or backticks.\n" +
            "- Return exactly one translated value.\n\n" +
            "SOURCE VALUE:\n" +
            "```\n" +
            sourceText +
            "\n```";

        using HttpClient client = CreateClient(localizationOptions.Value);
        string requestUri = BuildRequestUri(ollamaEndpoint);

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            requestUri,
            new
            {
                model,
                prompt,
                stream = false
            },
            cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using JsonDocument document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!document.RootElement.TryGetProperty("response", out JsonElement responseElement))
        {
            return null;
        }

        string? translated = responseElement.GetString();
        if (string.IsNullOrWhiteSpace(translated))
        {
            return null;
        }

        return SanitizeTranslatedValue(translated);
    }

    private static string? SanitizeTranslatedValue(string translated)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(translated);

        string value = translated.Trim();

        if (value.StartsWith("```", StringComparison.Ordinal))
        {
            int firstNewLineIndex = value.IndexOf('\n', StringComparison.Ordinal);
            if (firstNewLineIndex >= 0)
            {
                value = value[(firstNewLineIndex + 1)..];
            }

            if (value.EndsWith("```", StringComparison.Ordinal))
            {
                value = value[..^3];
            }

            value = value.Replace("```", string.Empty, StringComparison.Ordinal).Trim();
        }

        if (value.StartsWith("\"", StringComparison.Ordinal))
        {
            int firstNewLineIndex = value.IndexOf('\n', StringComparison.Ordinal);
            if (firstNewLineIndex >= 0)
            {
                value = value[(firstNewLineIndex + 1)..];
            }

            if (value.EndsWith("\"", StringComparison.Ordinal))
            {
                value = value[..^1];
            }

            value = value.Replace("\"", string.Empty, StringComparison.Ordinal).Trim();
        }

        if (value.StartsWith('`') && value.EndsWith('`') && value.Length > 1)
        {
            value = value[1..^1].Trim();
        }

        int noteMarkerIndex = value.IndexOf("(Note:", StringComparison.OrdinalIgnoreCase);
        if (noteMarkerIndex >= 0)
        {
            value = value[..noteMarkerIndex].TrimEnd();
        }

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static string ResolveLanguageName(string languageCode, string languageName)
    {
        if (!string.IsNullOrWhiteSpace(languageName))
        {
            return languageName.Trim();
        }

        try
        {
            return CultureInfo.GetCultureInfo(languageCode).DisplayName;
        }
        catch (CultureNotFoundException)
        {
            return languageCode;
        }
    }

    private static HttpClient CreateClient(LocalizationOllamaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        int timeoutSeconds = options.RequestTimeoutSeconds <= 0 ? 300 : options.RequestTimeoutSeconds;
        HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private static string BuildRequestUri(string ollamaEndpoint)
    {
        string baseEndpoint = ollamaEndpoint.Trim().TrimEnd('/');
        return $"{baseEndpoint}/api/generate";
    }

    private static string BuildShowRequestUri(string ollamaEndpoint)
    {
        string baseEndpoint = ollamaEndpoint.Trim().TrimEnd('/');
        return $"{baseEndpoint}/api/show";
    }

    private static string BuildPullRequestUri(string ollamaEndpoint)
    {
        string baseEndpoint = ollamaEndpoint.Trim().TrimEnd('/');
        return $"{baseEndpoint}/api/pull";
    }
}
