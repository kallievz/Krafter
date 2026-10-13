using System.Globalization;
using System.Reflection;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftAgeingMapper
{
    private static readonly CultureInfo SouthAfricanCulture = CultureInfo.GetCultureInfo("en-ZA");
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    public MunsoftAgeing Map(MunsoftAgeingRow row, int? instanceId = null, string? mdbCode = null)
    {
        ArgumentNullException.ThrowIfNull(row);

        MunsoftAgeing ageing = new MunsoftAgeing();
        MapRowValuesToEntity(row, ageing);

        if (instanceId.HasValue)
        {
            ageing.InstanceId = instanceId.Value;
        }

        if (!string.IsNullOrWhiteSpace(mdbCode) && string.IsNullOrWhiteSpace(ageing.MDBCode))
        {
            ageing.MDBCode = NormalizeString(mdbCode);
        }

        return ageing;
    }

    public MunsoftAgeingRow Map(MunsoftAgeing ageing)
    {
        ArgumentNullException.ThrowIfNull(ageing);

        MunsoftAgeingRow row = new MunsoftAgeingRow();
        MapEntityValuesToRow(ageing, row);
        return row;
    }

    public MunsoftAgeingRow Map(IReadOnlyDictionary<string, string?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        Dictionary<string, string?> normalizedValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string?> valuePair in values)
        {
            normalizedValues[NormalizeColumnName(valuePair.Key)] = valuePair.Value;
            normalizedValues[valuePair.Key] = valuePair.Value;
        }

        MunsoftAgeingRow row = new MunsoftAgeingRow();
        foreach (PropertyInfo property in typeof(MunsoftAgeingRow).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanWrite || property.PropertyType != typeof(string))
            {
                continue;
            }

            string columnName = NormalizeColumnName(property.Name);
            if (!normalizedValues.TryGetValue(columnName, out string? value) &&
                !normalizedValues.TryGetValue(property.Name, out value))
            {
                continue;
            }

            property.SetValue(row, NormalizeString(value));
        }

        return row;
    }

    public bool TryMap(MunsoftAgeingRow row, int rowNumber, int? instanceId, string? mdbCode, out MunsoftAgeing? ageing, out MunsoftAgeingLog? log)
    {
        try
        {
            ageing = Map(row, instanceId, mdbCode);
            log = null;
            return true;
        }
        catch (Exception exception)
        {
            ageing = null;
            log = new MunsoftAgeingLog
            {
                InstanceId = instanceId ?? 0,
                MDBCode = NormalizeString(mdbCode) ?? row.MDBCode,
                AccountNumber = row.AccountNumber,
                RowNumber = rowNumber,
                EntryJson = JsonSerializer.Serialize(row),
                ErrorMessage = exception.Message
            };

            return false;
        }
    }

    private static void MapRowValuesToEntity<TSource, TTarget>(TSource source, TTarget target)
    {
        Dictionary<string, PropertyInfo> sourceProperties = typeof(TSource)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
            .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo targetProperty in typeof(TTarget)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanWrite && property.GetIndexParameters().Length == 0))
        {
            if (!sourceProperties.TryGetValue(targetProperty.Name, out PropertyInfo? sourceProperty))
            {
                continue;
            }

            object? sourceValue = sourceProperty.GetValue(source);
            object? convertedValue = ConvertToTargetValue(targetProperty.PropertyType, sourceValue);
            if (convertedValue is null && IsNonNullableValueType(targetProperty.PropertyType))
            {
                continue;
            }

            targetProperty.SetValue(target, convertedValue);
        }
    }

    private static void MapEntityValuesToRow(MunsoftAgeing source, MunsoftAgeingRow target)
    {
        MapRowValuesToEntity(source, target);
    }

    private static object? ConvertToTargetValue(Type targetType, object? value)
    {
        Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value is null)
        {
            return nonNullableType.IsValueType ? Activator.CreateInstance(nonNullableType) : null;
        }

        string? stringValue = NormalizeString(value.ToString());
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            return nonNullableType.IsValueType ? Activator.CreateInstance(nonNullableType) : null;
        }

        if (nonNullableType == typeof(string))
        {
            return stringValue;
        }

        if (nonNullableType == typeof(int))
        {
            return TryParseInt32(stringValue, out int intValue)
                ? intValue
                : throw new FormatException($"Unable to parse '{stringValue}' as an integer.");
        }

        if (nonNullableType == typeof(decimal))
        {
            return TryParseDecimal(stringValue, out decimal decimalValue)
                ? decimal.Round(decimalValue, 2, MidpointRounding.AwayFromZero)
                : throw new FormatException($"Unable to parse '{stringValue}' as a decimal.");
        }

        if (nonNullableType == typeof(DateOnly))
        {
            return TryParseDateOnly(stringValue, out DateOnly dateOnly)
                ? dateOnly
                : throw new FormatException($"Unable to parse '{stringValue}' as a date.");
        }

        if (nonNullableType == typeof(DateTime))
        {
            return TryParseDateTime(stringValue, out DateTime dateTime)
                ? dateTime
                : throw new FormatException($"Unable to parse '{stringValue}' as a date time.");
        }

        if (nonNullableType == typeof(bool))
        {
            return TryParseBoolean(stringValue, out bool boolValue)
                ? boolValue
                : throw new FormatException($"Unable to parse '{stringValue}' as a boolean.");
        }

        if (nonNullableType.IsEnum)
        {
            return Enum.Parse(nonNullableType, stringValue, true);
        }

        return Convert.ChangeType(stringValue, nonNullableType, InvariantCulture);
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Any, SouthAfricanCulture, out result)
            || decimal.TryParse(value, NumberStyles.Any, InvariantCulture, out result);
    }

    private static bool TryParseDateOnly(string value, out DateOnly result)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", SouthAfricanCulture, DateTimeStyles.None, out result)
            || DateOnly.TryParse(value, SouthAfricanCulture, DateTimeStyles.None, out result)
            || DateOnly.TryParse(value, InvariantCulture, DateTimeStyles.None, out result);
    }

    private static bool TryParseDateTime(string value, out DateTime result)
    {
        return DateTime.TryParseExact(value, "yyyy-MM-dd", SouthAfricanCulture, DateTimeStyles.AssumeLocal, out result)
            || DateTime.TryParse(value, SouthAfricanCulture, DateTimeStyles.AssumeLocal, out result)
            || DateTime.TryParse(value, InvariantCulture, DateTimeStyles.AssumeLocal, out result);
    }

    private static bool TryParseBoolean(string value, out bool result)
    {
        if (bool.TryParse(value, out result))
        {
            return true;
        }

        if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "y", StringComparison.OrdinalIgnoreCase))
        {
            result = true;
            return true;
        }

        if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "no", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "n", StringComparison.OrdinalIgnoreCase))
        {
            result = false;
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryParseInt32(string value, out int result)
    {
        return int.TryParse(value, NumberStyles.Integer, InvariantCulture, out result);
    }

    private static string NormalizeColumnName(string value)
    {
        return new string(value.Where(character => character != '_' && character != '-' && character != ' ').ToArray());
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool IsNonNullableValueType(Type type)
    {
        Type nonNullableType = Nullable.GetUnderlyingType(type) ?? type;
        return nonNullableType.IsValueType && Nullable.GetUnderlyingType(type) is null;
    }
}
