using System.Reflection;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed class MunsoftDebtorParquetSerializer(MunsoftDebtorMapper mapper)
{
    private static readonly Type SerializerType = ResolveType("Parquet.Serialization.ParquetSerializer");
    private static readonly Type ParquetOptionsType = ResolveType("Parquet.ParquetOptions");

    private readonly MunsoftDebtorMapper mapper = mapper;

    public async Task<List<MunsoftDebtorRow>> ReadRowsAsync(Stream stream)
    {
        object? result = await InvokeParquetAsync(
                SerializerType,
                "DeserializeAsync",
                typeof(MunsoftDebtorRow),
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);

        if (result is null)
        {
            return new List<MunsoftDebtorRow>();
        }

        if (result is IEnumerable<MunsoftDebtorRow> rows)
        {
            return rows.ToList();
        }

        PropertyInfo? dataProperty = result.GetType().GetProperty("Data");
        if (dataProperty?.GetValue(result) is IEnumerable<MunsoftDebtorRow> typedRows)
        {
            return typedRows.ToList();
        }

        throw new InvalidOperationException($"Unexpected Parquet.Net deserialization result type: {result.GetType().FullName}");
    }

    public async Task<MunsoftDebtorImportResult> ReadAsync(Stream stream, int? instanceId = null, string? mdbCode = null)
    {
        List<MunsoftDebtorRow> rows = await ReadRowsAsync(stream).ConfigureAwait(false);
        List<MunsoftDebtor> debtors = new List<MunsoftDebtor>(rows.Count);
        List<MunsoftDebtorLog> logs = new List<MunsoftDebtorLog>();
        HashSet<string> uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < rows.Count; index++)
        {
            if (mapper.TryMap(rows[index], index + 1, instanceId, mdbCode, out MunsoftDebtor? debtor, out MunsoftDebtorLog? log))
            {
                if (debtor is not null)
                {
                    string uniqueKey = BuildUniqueKey(debtor.InstanceId, debtor.AccountNumber);
                    if (!uniqueKeys.Add(uniqueKey))
                    {
                        logs.Add(new MunsoftDebtorLog
                        {
                            InstanceId = debtor.InstanceId,
                            MDBCode = debtor.MDBCode,
                            AccountNumber = debtor.AccountNumber,
                            RowNumber = index + 1,
                            EntryJson = JsonSerializer.Serialize(rows[index]),
                            ErrorMessage = "Duplicate debtor key in import stream."
                        });
                        continue;
                    }

                    debtors.Add(debtor);
                }
            }
            else if (log is not null)
            {
                logs.Add(log);
            }
        }

        return new MunsoftDebtorImportResult(debtors, logs);
    }

    public async Task WriteRowsAsync(IEnumerable<MunsoftDebtorRow> rows, Stream stream)
    {
        await InvokeParquetAsync(
                SerializerType,
                "SerializeAsync",
                typeof(MunsoftDebtorRow),
                rows,
                stream,
                CreateOptions(),
                null,
                CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task WriteAsync(IEnumerable<MunsoftDebtor> debtors, Stream stream)
    {
        IEnumerable<MunsoftDebtorRow> rows = debtors.Select(mapper.Map);
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
