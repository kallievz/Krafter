namespace AditiKraft.Krafter.Backend.Infrastructure.Localization;

public sealed class LocalizationOllamaOptions
{
    public const string SectionName = "Localization:Ollama";

    public bool Enabled { get; set; } = false;
    public string Endpoint { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "llama3.1";
    public int RequestTimeoutSeconds { get; set; } = 300;
}
