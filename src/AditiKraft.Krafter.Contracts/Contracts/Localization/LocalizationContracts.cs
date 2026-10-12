namespace AditiKraft.Krafter.Contracts.Contracts.Localization;

public sealed class LanguageDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public sealed class CreateLanguageRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class UpdateLanguageRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class ResourceDto
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public sealed class CreateResourceRequest
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class UpdateResourceRequest
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class ResourceTranslationDto
{
    public string Id { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string LanguageId { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ResourceDto? Resource { get; set; }
    public LanguageDto? Language { get; set; }
}

public sealed class CreateResourceTranslationRequest
{
    public string ResourceId { get; set; } = string.Empty;
    public string LanguageId { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class UpdateResourceTranslationRequest
{
    public string ResourceId { get; set; } = string.Empty;
    public string LanguageId { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class LocalizeTextRequest
{
    public string Key { get; set; } = string.Empty;
    public string FallbackValue { get; set; } = string.Empty;
    public List<string> Arguments { get; set; } = [];
}

public sealed class LocalizeBatchRequest
{
    public List<LocalizeTextRequest> Entries { get; set; } = [];
}

public sealed class LocalizedTextDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = "en";
    public bool UsedFallback { get; set; }
    public bool AutoCreatedEnglishResource { get; set; }
    public bool AutoCreatedEnglishTranslation { get; set; }
}
