using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AditiKraft.Krafter.Backend.Features.Localization.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static partial class LocalizationResourceSeedData
{
    private static readonly string _radzenResxPath = Path.Combine(
        AppContext.BaseDirectory,
        "Infrastructure", "Persistence", "Seeding", "RadzenStrings.resx");

    private static ILogger? _logger;

    public static IEnumerable<Resource> GetResources(ILogger? logger = null)
    {
        _logger = logger;

        List<Resource> resources = [];
        HashSet<string> existingKeys = new(StringComparer.OrdinalIgnoreCase);
        IEnumerable<Type> entityTypes = GetEntityTypes();

        _logger?.LogInformation("Starting localization resource discovery. RadzenStrings.resx path: {Path}", _radzenResxPath);
        _logger?.LogInformation("Discovered {Count} seedable entity types", entityTypes.Count());

        // Add entity property resources
        //int entityPropertyCount = 0;
        foreach (Type entityType in entityTypes)
        {
            string key = $"Entity_{Pluralize(entityType.Name)}";
            if (!existingKeys.Contains(key))
            {
                resources.Add(new Resource
                {
                    Id = Guid.CreateVersion7().ToString(),
                    Key = key,
                    Description = $"Entity {Pluralize(entityType.Name)}"
                });
                existingKeys.Add(key);
            }
            foreach (string propertyName in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(property => IsSeedableProperty(property.PropertyType))
                         .Select(property => property.Name)
                         .OrderBy(propertyName => propertyName))
            {
                key = $"{entityType.Name}_{propertyName}";
                if (!existingKeys.Contains(key))
                {
                    resources.Add(new Resource
                    {
                        Id = Guid.CreateVersion7().ToString(),
                        Key = key,
                        Description = $"{entityType.Name} {SplitCamelCase(propertyName)}"
                    });
                    existingKeys.Add(key);
                }
            }
        }

        // Add Radzen component strings
        IEnumerable<Resource> radzenResources = GetRadzenResources();
        _logger?.LogInformation("Discovered {Count} Radzen resource strings", radzenResources.Count());
        foreach (Resource radzenResource in radzenResources)
        {
            if (!existingKeys.Contains(radzenResource.Key))
            {
                resources.Add(radzenResource);
                existingKeys.Add(radzenResource.Key);
            }
        }

        // Add standard Button_ keys
        IEnumerable<Resource> buttonKeys = GetStandardButtonKeys();
        foreach (Resource buttonResource in buttonKeys)
        {
            if (!existingKeys.Contains(buttonResource.Key))
            {
                resources.Add(buttonResource);
                existingKeys.Add(buttonResource.Key);
            }
        }

        // Add standard Nav_ keys
        IEnumerable<Resource> navKeys = GetStandardNavKeys();
        foreach (Resource navResource in navKeys)
        {
            if (!existingKeys.Contains(navResource.Key))
            {
                resources.Add(navResource);
                existingKeys.Add(navResource.Key);
            }
        }

        _logger?.LogInformation("Resource discovery complete. Total resources to seed: {Count}", resources.Count);
        return resources;
    }

    /// <summary>
    /// Get standard Button_ action keys.
    /// </summary>
    private static IEnumerable<Resource> GetStandardButtonKeys()
    {
        string[] buttons = new[]
        {
            "Save", "Delete", "Cancel", "Submit", "Reset", "Create", "Update",
            "Add", "Remove", "Edit", "View", "Close", "Export", "Import", "Search",
            "Filter", "Clear", "Apply", "Refresh", "Back", "Next", "Previous",
            "Download", "Upload", "Approve", "Reject", "Confirm", "Yes", "No", "Sync"
        };

        return buttons.Select(action => new Resource
        {
            Id = Guid.CreateVersion7().ToString(),
            Key = $"Button_{action}",
            Description = $"{action}"
        }).ToList();
    }

    /// <summary>
    /// Get standard Nav_ menu keys discovered from entity names.
    /// </summary>
    private static IEnumerable<Resource> GetStandardNavKeys()
    {
        // Discover entity-based navigation items
        IEnumerable<Type> entityTypes = GetEntityTypes();
        var navKeys = new List<Resource>();

        foreach (Type entityType in entityTypes)
        {
            string pluralName = Pluralize(entityType.Name);
            string navKey = $"Nav_{pluralName}";
            navKeys.Add(new Resource
            {
                Id = Guid.CreateVersion7().ToString(),
                Key = navKey,
                Description = $"{SplitCamelCase(pluralName)}"
            });
        }

        // Add common navigation items
        var commonNavItems = new[]
        {
            "Home", "Dashboard", "Settings", "Users", "Profile", "Help", "About"
        };

        foreach (string? item in commonNavItems)
        {
            navKeys.Add(new Resource
            {
                Id = Guid.CreateVersion7().ToString(),
                Key = $"Nav_{item}",
                Description = $"{item}"
            });
        }

        return navKeys;
    }

    public static IEnumerable<ResourceTranslation> GetTranslations(IEnumerable<Resource> resources, IEnumerable<Language> languages)
    {
        List<ResourceTranslation> translations = [];
        Dictionary<string, string> radzenTranslations = GetRadzenTranslations()
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (Resource resource in resources)
        {
            string value;

            // Check if this is a Radzen resource by looking it up in radzen translations
            if (radzenTranslations.TryGetValue(resource.Key, out string? radzenValue))
            {
                value = radzenValue ?? string.Empty;
            }
            // Check if this is a standard Button_ key
            else if (resource.Key.StartsWith("Button_", StringComparison.OrdinalIgnoreCase))
            {
                string actionName = resource.Key["Button_".Length..];
                value = SplitCamelCase(actionName);
            }
            // Check if this is a standard Nav_ key
            else if (resource.Key.StartsWith("Nav_", StringComparison.OrdinalIgnoreCase))
            {
                string navName = resource.Key["Nav_".Length..];
                value = SplitCamelCase(navName);
            }
            // Regular entity property: extract name and split camel case
            else
            {
                string propertyName = resource.Key.Contains('_') ? resource.Key[(resource.Key.IndexOf('_') + 1)..] : resource.Key;
                value = SplitCamelCase(propertyName);
            }

            foreach (Language language in languages)
            {
                translations.Add(new ResourceTranslation
                {
                    Id = Guid.CreateVersion7().ToString(),
                    ResourceId = resource.Id,
                    LanguageId = language.Id,
                    Value = value
                });
            }
        }

        return translations;
    }

    /// <summary>
    /// Loads all Radzen resource strings from RadzenStrings.resx.
    /// Keys and values are used exactly as they appear in the resx file.
    /// </summary>
    private static IEnumerable<Resource> GetRadzenResources()
    {
        if (!File.Exists(_radzenResxPath))
        {
            _logger?.LogWarning("RadzenStrings.resx file not found at: {Path}", _radzenResxPath);
            return [];
        }

        try
        {
            _logger?.LogDebug("Loading Radzen resources from: {Path}", _radzenResxPath);
            XDocument doc = XDocument.Load(_radzenResxPath);

            var resources = doc.Descendants("data")
                .Select(element => new
                {
                    Name = element.Attribute("name")?.Value,
                    Value = element.Element("value")?.Value
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => new Resource
                {
                    Id = Guid.CreateVersion7().ToString(),
                    Key = x.Name!,
                    Description = $"Radzen: {x.Name}"
                })
                .ToList();

            _logger?.LogDebug("Successfully loaded {Count} Radzen resources", resources.Count);
            return resources;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load Radzen resources from {Path}", _radzenResxPath);
            return [];
        }
    }

    /// <summary>
    /// Loads all Radzen translations from RadzenStrings.resx for EN locale.
    /// Values are preserved exactly as-is from the resource file.
    /// </summary>
    private static IEnumerable<(string Key, string Value)> GetRadzenTranslations()
    {
        if (!File.Exists(_radzenResxPath))
        {
            _logger?.LogWarning("RadzenStrings.resx file not found at: {Path}", _radzenResxPath);
            return [];
        }

        try
        {
            _logger?.LogDebug("Loading Radzen translations from: {Path}", _radzenResxPath);
            XDocument doc = XDocument.Load(_radzenResxPath);

            var translations = doc.Descendants("data")
                .Select(element => new
                {
                    Name = element.Attribute("name")?.Value,
                    Value = element.Element("value")?.Value
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => (x.Name!, x.Value!))
                .ToList();

            _logger?.LogDebug("Successfully loaded {Count} Radzen translations", translations.Count);
            return translations;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load Radzen translations from {Path}", _radzenResxPath);
            return [];
        }
    }

    private static IEnumerable<Type> GetEntityTypes()
    {
        return typeof(Resource).Assembly
            .GetTypes()
            .Where(type => type.Namespace?.StartsWith("AditiKraft.Krafter.Backend.Features.") == true
                        && type is { IsClass: true, IsAbstract: false }
                        && type.Namespace.EndsWith(".Common", StringComparison.OrdinalIgnoreCase)
                        && type.Name != nameof(Object)
                        && !IsCompilerGenerated(type)
                        && !IsNonEntityClass(type))
            .OrderBy(type => type.Name)
            .ToList();
    }

    /// <summary>
    /// Check if a type is compiler-generated (async state machines, display classes, etc.)
    /// </summary>
    private static bool IsCompilerGenerated(Type type)
    {
        // Compiler-generated types typically have angle brackets or special patterns
        return type.Name.Contains('<') || type.Name.Contains('>') || type.Name.StartsWith("__");
    }

    /// <summary>
    /// Check if a type is a service, helper, or other non-entity class.
    /// Entity classes are typically data models, not services.
    /// </summary>
    private static bool IsNonEntityClass(Type type)
    {
        // Exclude nested/inner classes (they typically aren't domain entities for localization)
        if (type.IsNested)
        {
            return true;
        }

        // Exclude services (typically end with "Service")
        if (type.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Services", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Exclude helpers, handlers, factories, repositories, etc.
        if (type.Name.EndsWith("Helper", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Helpers", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Handler", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Handlers", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Factory", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Factories", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Repositories", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Manager", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Managers", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Builder", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Builders", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Exclude cache, resolver, and other utility classes
        if (type.Name.Contains("Cache", StringComparison.OrdinalIgnoreCase) ||
            type.Name.Contains("Resolver", StringComparison.OrdinalIgnoreCase) ||
            type.Name.Contains("Converter", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Dtos", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Options", StringComparison.OrdinalIgnoreCase) ||
            type.Name.EndsWith("Settings", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }


    private static bool IsSeedableProperty(Type type)
    {
        return type == typeof(string)
            || type == typeof(bool)
            || type == typeof(int)
            || type == typeof(long);
    }

    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        string result = CamelCaseRegex().Replace(input, " $1");
        return result.Trim();
    }

    /// <summary>
    /// Pluralize an English word using common rules.
    /// </summary>
    private static string Pluralize(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return word;
        }

        // Words ending in 's', 'x', 'z', 'ch', 'sh' -> add 'es'
        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
        {
            return word + "es";
        }

        // Words ending in consonant + 'y' -> replace 'y' with 'ies'
        if (word.Length >= 2 && word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            !IsVowel(word[word.Length - 2]))
        {
            return word[..^1] + "ies";
        }

        // Words ending in 'f' or 'fe' -> replace with 'ves'
        if (word.EndsWith("fe", StringComparison.OrdinalIgnoreCase))
        {
            return word[..^2] + "ves";
        }

        if (word.EndsWith("f", StringComparison.OrdinalIgnoreCase))
        {
            return word[..^1] + "ves";
        }

        // Words ending in consonant + 'o' -> add 'es'
        if (word.Length >= 2 && word.EndsWith("o", StringComparison.OrdinalIgnoreCase) &&
            !IsVowel(word[word.Length - 2]))
        {
            return word + "es";
        }

        // Default: add 's'
        return word + "s";
    }

    private static bool IsVowel(char c)
    {
        return "aeiouAEIOU".Contains(c);
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex CamelCaseRegex();
}
