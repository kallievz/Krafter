using System.Reflection;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftAgeingParquetSerializer(MunsoftAgeingMapper mapper)
{
    private static readonly Type SerializerType = ResolveType("Parquet.Serialization.ParquetSerializer");
    private static readonly Type ParquetOptionsType = ResolveType("Parquet.ParquetOptions");

    private readonly MunsoftAgeingMapper mapper = mapper;

    public async Task<List<MunsoftAgeingRow>> ReadRowsAsync(Stream stream)
    {
        object? result = await InvokeParquetAsync(
                SerializerType,
                "DeserializeAsync",
                typeof(MunsoftAgeingRow),
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);

        if (result is null)
        {
            return new List<MunsoftAgeingRow>();
        }

        if (result is IEnumerable<MunsoftAgeingRow> rows)
        {
            return rows.ToList();
        }

        PropertyInfo? dataProperty = result.GetType().GetProperty("Data");
        if (dataProperty?.GetValue(result) is IEnumerable<MunsoftAgeingRow> typedRows)
        {
            return typedRows.ToList();
        }

        throw new InvalidOperationException($"Unexpected Parquet.Net deserialization result type: {result.GetType().FullName}");
    }

    public async Task<MunsoftAgeingImportResult> ReadAsync(Stream stream, int? instanceId = null, string? mdbCode = null)
    {
        List<MunsoftAgeingRow> rows = await ReadRowsAsync(stream).ConfigureAwait(false);
        List<MunsoftAgeing> ageings = new List<MunsoftAgeing>(rows.Count);
        List<MunsoftAgeingLog> logs = new List<MunsoftAgeingLog>();
        HashSet<string> uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < rows.Count; index++)
        {
            if (mapper.TryMap(rows[index], index + 1, instanceId, mdbCode, out MunsoftAgeing? ageing, out MunsoftAgeingLog? log))
            {
                if (ageing is not null)
                {
                    string uniqueKey = BuildUniqueKey(ageing.InstanceId, ageing.AccountNumber);
                    if (!uniqueKeys.Add(uniqueKey))
                    {
                        logs.Add(new MunsoftAgeingLog
                        {
                            InstanceId = ageing.InstanceId,
                            MDBCode = ageing.MDBCode,
                            AccountNumber = ageing.AccountNumber,
                            RowNumber = index + 1,
                            EntryJson = JsonSerializer.Serialize(rows[index]),
                            ErrorMessage = "Duplicate ageing key in import stream."
                        });
                        continue;
                    }

                    ageings.Add(ageing);
                }
            }
            else if (log is not null)
            {
                logs.Add(log);
            }
        }

        return new MunsoftAgeingImportResult(ageings, logs);
    }

    public async Task WriteRowsAsync(IEnumerable<MunsoftAgeingRow> rows, Stream stream)
    {
        await InvokeParquetAsync(
                SerializerType,
                "SerializeAsync",
                typeof(MunsoftAgeingRow),
                rows,
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task WriteAsync(IEnumerable<MunsoftAgeing> ageings, Stream stream)
    {
        IEnumerable<MunsoftAgeingRow> rows = ageings.Select(mapper.Map);
        await WriteRowsAsync(rows, stream).ConfigureAwait(false);
    }

    private static async Task<object?> InvokeParquetAsync(Type serializerType, string methodName, Type itemType, params object?[] arguments)
    {
        MethodInfo methodInfo = serializerType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == methodName
                && method.IsGenericMethodDefinition
                && method.GetParameters().Length == arguments.Length);

        MethodInfo genericMethod = methodInfo.MakeGenericMethod(itemType);
        object? taskObject = genericMethod.Invoke(null, arguments);
        if (taskObject is not Task task)
        {
            throw new InvalidOperationException($"Parquet.Net method '{methodName}' did not return a task.");
        }

        await task.ConfigureAwait(false);
        PropertyInfo? resultProperty = taskObject.GetType().GetProperty("Result");
        return resultProperty?.GetValue(taskObject);
    }

    private static object CreateOptions()
    {
        object? options = Activator.CreateInstance(ParquetOptionsType);
        return options ?? throw new InvalidOperationException("Unable to create Parquet options.");
    }

    private static string BuildUniqueKey(int instanceId, string? accountNumber)
    {
        string normalizedAccountNumber = NormalizeKeyPart(accountNumber);
        return $"{instanceId}|{normalizedAccountNumber}";
    }

    private static string NormalizeKeyPart(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }

    private static Type ResolveType(string typeName)
    {
        Type? type = Type.GetType($"{typeName}, Parquet");
        return type ?? throw new InvalidOperationException($"Unable to locate Parquet type '{typeName}'.");
    }
}
