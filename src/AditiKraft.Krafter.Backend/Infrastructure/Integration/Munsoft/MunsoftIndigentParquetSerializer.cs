using System.Reflection;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftIndigentParquetSerializer(MunsoftIndigentMapper mapper)
{
    private static readonly Type SerializerType = ResolveType("Parquet.Serialization.ParquetSerializer");
    private static readonly Type ParquetOptionsType = ResolveType("Parquet.ParquetOptions");

    private readonly MunsoftIndigentMapper mapper = mapper;

    public async Task<List<MunsoftIndigentRow>> ReadRowsAsync(Stream stream)
    {
        object? result = await InvokeParquetAsync(
                SerializerType,
                "DeserializeAsync",
                typeof(MunsoftIndigentRow),
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);

        if (result is null)
        {
            return new List<MunsoftIndigentRow>();
        }

        if (result is IEnumerable<MunsoftIndigentRow> rows)
        {
            return rows.ToList();
        }

        PropertyInfo? dataProperty = result.GetType().GetProperty("Data");
        if (dataProperty?.GetValue(result) is IEnumerable<MunsoftIndigentRow> typedRows)
        {
            return typedRows.ToList();
        }

        throw new InvalidOperationException($"Unexpected Parquet.Net deserialization result type: {result.GetType().FullName}");
    }

    public async Task<MunsoftIndigentImportResult> ReadAsync(Stream stream, int? instanceId = null, string? mdbCode = null)
    {
        List<MunsoftIndigentRow> rows = await ReadRowsAsync(stream).ConfigureAwait(false);
        List<MunsoftIndigent> indigents = new List<MunsoftIndigent>(rows.Count);
        List<MunsoftIndigentLog> logs = new List<MunsoftIndigentLog>();
        HashSet<string> uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < rows.Count; index++)
        {
            if (mapper.TryMap(rows[index], index + 1, instanceId, mdbCode, out MunsoftIndigent? indigent, out MunsoftIndigentLog? log))
            {
                if (indigent is not null)
                {
                    string uniqueKey = BuildUniqueKey(indigent.InstanceId, indigent.AccountNumber, indigent.IndigentSequenceNumber);
                    if (!uniqueKeys.Add(uniqueKey))
                    {
                        logs.Add(new MunsoftIndigentLog
                        {
                            InstanceId = indigent.InstanceId,
                            MDBCode = indigent.MDBCode,
                            AccountNumber = indigent.AccountNumber,
                            IndigentSequenceNumber = indigent.IndigentSequenceNumber,
                            RowNumber = index + 1,
                            EntryJson = JsonSerializer.Serialize(rows[index]),
                            ErrorMessage = "Duplicate indigent key in import stream."
                        });
                        continue;
                    }

                    indigents.Add(indigent);
                }
            }
            else if (log is not null)
            {
                logs.Add(log);
            }
        }

        return new MunsoftIndigentImportResult(indigents, logs);
    }

    public async Task WriteRowsAsync(IEnumerable<MunsoftIndigentRow> rows, Stream stream)
    {
        await InvokeParquetAsync(
                SerializerType,
                "SerializeAsync",
                typeof(MunsoftIndigentRow),
                rows,
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task WriteAsync(IEnumerable<MunsoftIndigent> indigents, Stream stream)
    {
        IEnumerable<MunsoftIndigentRow> rows = indigents.Select(mapper.Map);
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

    private static string BuildUniqueKey(int instanceId, string? accountNumber, string? indigentSequenceNumber)
    {
        string normalizedAccountNumber = NormalizeKeyPart(accountNumber);
        string normalizedIndigentSequenceNumber = NormalizeKeyPart(indigentSequenceNumber);
        return $"{instanceId}|{normalizedAccountNumber}|{normalizedIndigentSequenceNumber}";
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
