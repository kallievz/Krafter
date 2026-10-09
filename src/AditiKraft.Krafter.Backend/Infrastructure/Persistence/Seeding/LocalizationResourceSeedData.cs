using System.Reflection;
using System.Text.RegularExpressions;
using AditiKraft.Krafter.Backend.Features.Localization.Common;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static partial class LocalizationResourceSeedData
{

    public static IEnumerable<Resource> GetResources()
    {
        List<Resource> resources = [];
        IEnumerable<Type> entityTypes = GetEntityTypes();

        foreach (Type entityType in entityTypes)
        {
            foreach (string propertyName in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(property => IsSeedableProperty(property.PropertyType))
                         .Select(property => property.Name)
                         .OrderBy(propertyName => propertyName))
            {
                resources.Add(new Resource
                {
                    Id = Guid.CreateVersion7().ToString(),
                    Key = $"{entityType.Name}_{propertyName}",
                    Description = $"{entityType.Name} {SplitCamelCase(propertyName)}"
                });
            }
        }

        return resources;
    }

    public static IEnumerable<ResourceTranslation> GetTranslations(IEnumerable<Resource> resources, IEnumerable<Language> languages)
    {
        List<ResourceTranslation> translations = [];

        foreach (Resource resource in resources)
        {
            string propertyName = resource.Key[(resource.Key.IndexOf('_') + 1)..];
            string value = SplitCamelCase(propertyName);

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

    private static IEnumerable<Type> GetEntityTypes()
    {
        Assembly assembly = typeof(LocalizationResourceSeedData).Assembly;

        return assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(DbContext).IsAssignableFrom(type))
            .SelectMany(dbContextType => dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(property => property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(property => property.PropertyType.GetGenericArguments()[0])
            .Where(entityType => entityType.Assembly == assembly)
            .Distinct()
            .OrderBy(entityType => entityType.Name);
    }

    private static bool IsSeedableProperty(Type propertyType)
    {
        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlyingType == typeof(string) || underlyingType == typeof(bool) || underlyingType == typeof(int) ||
            underlyingType == typeof(long) || underlyingType == typeof(decimal) || underlyingType == typeof(double) ||
            underlyingType == typeof(float) || underlyingType == typeof(DateTime) || underlyingType == typeof(Guid))
        {
            return true;
        }

        return underlyingType.IsEnum;
    }

    private static string SplitCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return CamelCaseSplitterRegex().Replace(value, " $1").Trim();
    }

    [GeneratedRegex("([A-Z])")]
    private static partial Regex CamelCaseSplitterRegex();
}
