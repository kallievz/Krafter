using System.Globalization;
using System.Reflection;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftDebtorMapper
{
    private static readonly CultureInfo SouthAfricanCulture = CultureInfo.GetCultureInfo("en-ZA");
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    public MunsoftDebtor Map(MunsoftDebtorRow row, int? instanceId = null, string? mdbCode = null)
    {
        ArgumentNullException.ThrowIfNull(row);

        MunsoftDebtor debtor = new MunsoftDebtor();
        MapRowValuesToEntity(row, debtor);

        if (instanceId.HasValue)
        {
            debtor.InstanceId = instanceId.Value;
        }

        if (!string.IsNullOrWhiteSpace(mdbCode) && string.IsNullOrWhiteSpace(debtor.MDBCode))
        {
            debtor.MDBCode = NormalizeString(mdbCode);
        }

        return debtor;
    }

    public MunsoftDebtorRow Map(MunsoftDebtor debtor)
    {
        ArgumentNullException.ThrowIfNull(debtor);

        MunsoftDebtorRow row = new MunsoftDebtorRow();
        MapEntityValuesToRow(debtor, row);
        return row;
    }

    public MunsoftDebtorRow Map(IReadOnlyDictionary<string, string?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        Dictionary<string, string?> normalizedValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string?> valuePair in values)
        {
            normalizedValues[NormalizeColumnName(valuePair.Key)] = valuePair.Value;
            normalizedValues[valuePair.Key] = valuePair.Value;
        }

        MunsoftDebtorRow row = new MunsoftDebtorRow();
        foreach (PropertyInfo property in typeof(MunsoftDebtorRow).GetProperties(BindingFlags.Instance | BindingFlags.Public))
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

    public bool TryMap(MunsoftDebtorRow row, int rowNumber, int? instanceId, string? mdbCode, out MunsoftDebtor? debtor, out MunsoftDebtorLog? log)
    {
        try
        {
            debtor = Map(row, instanceId, mdbCode);
            log = null;
            return true;
        }
        catch (Exception exception)
        {
            debtor = null;
            log = new MunsoftDebtorLog
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
                : throw new FormatException($"Unable to parse '{stringValue}' as a date/time.");
        }

        if (nonNullableType == typeof(bool))
        {
            return TryParseBoolean(stringValue, out bool boolValue)
                ? boolValue
                : throw new FormatException($"Unable to parse '{stringValue}' as a boolean.");
        }

        return Convert.ChangeType(stringValue, nonNullableType, InvariantCulture);
    }

    private static string? ConvertToStringValue(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return value switch
        {
            string stringValue => NormalizeString(stringValue),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", InvariantCulture),
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd", InvariantCulture),
            decimal decimalValue => decimal.Round(decimalValue, 2, MidpointRounding.AwayFromZero).ToString("0.00", SouthAfricanCulture),
            bool boolValue => boolValue ? "true" : "false",
            _ => Convert.ToString(value, InvariantCulture)
        };
    }

    private static void MapEntityValuesToRow<TSource, TTarget>(TSource source, TTarget target)
    {
        Dictionary<string, PropertyInfo> targetProperties = typeof(TTarget)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanWrite && property.GetIndexParameters().Length == 0)
            .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo sourceProperty in typeof(TSource)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead && property.GetIndexParameters().Length == 0))
        {
            if (!targetProperties.TryGetValue(sourceProperty.Name, out PropertyInfo? targetProperty))
            {
                continue;
            }

            object? sourceValue = sourceProperty.GetValue(source);
            string? convertedValue = ConvertToStringValue(sourceValue);
            if (convertedValue is null && targetProperty.PropertyType == typeof(string))
            {
                continue;
            }

            targetProperty.SetValue(target, convertedValue);
        }
    }

    private static bool TryParseDateOnly(string value, out DateOnly dateOnly)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", InvariantCulture, DateTimeStyles.None, out dateOnly)
            || DateOnly.TryParse(value, SouthAfricanCulture, DateTimeStyles.None, out dateOnly)
            || DateOnly.TryParse(value, InvariantCulture, DateTimeStyles.None, out dateOnly);
    }

    private static bool TryParseDateTime(string value, out DateTime dateTime)
    {
        return DateTime.TryParseExact(value, "yyyy-MM-dd", InvariantCulture, DateTimeStyles.None, out dateTime)
            || DateTime.TryParse(value, SouthAfricanCulture, DateTimeStyles.None, out dateTime)
            || DateTime.TryParse(value, InvariantCulture, DateTimeStyles.None, out dateTime);
    }

    private static bool TryParseInt32(string value, out int intValue)
    {
        return int.TryParse(value, NumberStyles.Integer, InvariantCulture, out intValue);
    }

    private static bool TryParseDecimal(string value, out decimal decimalValue)
    {
        return decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, SouthAfricanCulture, out decimalValue)
            || decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, InvariantCulture, out decimalValue);
    }

    private static bool TryParseBoolean(string value, out bool boolValue)
    {
        if (bool.TryParse(value, out boolValue))
        {
            return true;
        }

        if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "Y", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "YES", StringComparison.OrdinalIgnoreCase))
        {
            boolValue = true;
            return true;
        }

        if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "N", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "NO", StringComparison.OrdinalIgnoreCase))
        {
            boolValue = false;
            return true;
        }

        boolValue = false;
        return false;
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeColumnName(string columnName)
    {
        return columnName.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
    }

    private static bool IsNonNullableValueType(Type type)
    {
        Type nonNullableType = Nullable.GetUnderlyingType(type) ?? type;
        return nonNullableType.IsValueType && Nullable.GetUnderlyingType(type) is null;
    }
}
