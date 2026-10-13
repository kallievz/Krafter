using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

namespace AditiKraft.Krafter.Tests.Integration;

public sealed class MunsoftAgeingParquetSerializerTests
{
    [Fact]
    public void Map_FromDictionary_UsesSnakeCaseAndPreservesStringColumns()
    {
        MunsoftAgeingMapper mapper = new MunsoftAgeingMapper();

        MunsoftAgeingRow row = mapper.Map(new Dictionary<string, string?>
        {
            ["account_number"] = "12345",
            ["current_month"] = "2026-10",
            ["due_date"] = "2026-10-08",
            ["days_30"] = "12.345",
            ["total_outstanding"] = "98.765"
        });

        Assert.Equal("12345", row.AccountNumber);
        Assert.Equal("2026-10", row.CurrentMonth);
        Assert.Equal("2026-10-08", row.DueDate);
        Assert.Equal("12.345", row.Days30);
        Assert.Equal("98.765", row.TotalOutstanding);
    }

    [Fact]
    public void Map_RowToEntity_ConvertsDateAndDecimalValues()
    {
        MunsoftAgeingMapper mapper = new MunsoftAgeingMapper();
        MunsoftAgeingRow row = new MunsoftAgeingRow
        {
            MDBCode = "MDB-1",
            AccountNumber = "A-100",
            CurrentMonth = "2026-10",
            DueDate = "2026-10-08",
            Days30 = "12.345",
            Days60 = "13.1",
            TotalOutstanding = "25.555"
        };

        MunsoftAgeing ageing = mapper.Map(row, instanceId: 42, mdbCode: "MDB-1");

        Assert.Equal(42, ageing.InstanceId);
        Assert.Equal("MDB-1", ageing.MDBCode);
        Assert.Equal("A-100", ageing.AccountNumber);
        Assert.Equal(new DateOnly(2026, 10, 8), ageing.DueDate);
        Assert.Equal(12.35m, ageing.Days30);
        Assert.Equal(13.10m, ageing.Days60);
        Assert.Equal(25.56m, ageing.TotalOutstanding);
    }

    [Fact]
    public void TryMap_InvalidDecimal_CreatesFailureLog()
    {
        MunsoftAgeingMapper mapper = new MunsoftAgeingMapper();
        MunsoftAgeingRow row = new MunsoftAgeingRow
        {
            AccountNumber = "A-100",
            DueDate = "2026-10-08",
            Days30 = "not-a-number"
        };

        bool mapped = mapper.TryMap(row, rowNumber: 7, instanceId: 42, mdbCode: "MDB-1", out MunsoftAgeing? ageing, out MunsoftAgeingLog? log);

        Assert.False(mapped);
        Assert.Null(ageing);
        Assert.NotNull(log);
        Assert.Equal(42, log!.InstanceId);
        Assert.Equal("MDB-1", log.MDBCode);
        Assert.Equal("A-100", log.AccountNumber);
        Assert.Equal(7, log.RowNumber);
        Assert.Contains("decimal", log.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
