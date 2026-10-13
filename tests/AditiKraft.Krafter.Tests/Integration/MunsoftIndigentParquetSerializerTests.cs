using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

namespace AditiKraft.Krafter.Tests.Integration;

public sealed class MunsoftIndigentParquetSerializerTests
{
    [Fact]
    public void Map_FromDictionary_UsesSnakeCaseAndPreservesStringColumns()
    {
        MunsoftIndigentMapper mapper = new MunsoftIndigentMapper();

        MunsoftIndigentRow row = mapper.Map(new Dictionary<string, string?>
        {
            ["account_number"] = "12345",
            ["indigent_sequence_number"] = "SEQ-1",
            ["application_date"] = "2026-10-08",
            ["approved"] = "true",
            ["number_of_occupants"] = "5",
            ["indigent_income"] = "1200.556"
        });

        Assert.Equal("12345", row.AccountNumber);
        Assert.Equal("SEQ-1", row.IndigentSequenceNumber);
        Assert.Equal("2026-10-08", row.ApplicationDate);
        Assert.Equal("true", row.Approved);
        Assert.Equal("5", row.NumberOfOccupants);
        Assert.Equal("1200.556", row.IndigentIncome);
    }

    [Fact]
    public void Map_RowToEntity_ConvertsDateDecimalBoolAndLongValues()
    {
        MunsoftIndigentMapper mapper = new MunsoftIndigentMapper();
        MunsoftIndigentRow row = new MunsoftIndigentRow
        {
            MDBCode = "MDB-1",
            AccountNumber = "A-100",
            IndigentSequenceNumber = "SEQ-1",
            ApplicationDate = "2026-10-08",
            Approved = "yes",
            Active = "1",
            NumberOfOccupants = "7",
            IndigentIncome = "123.456",
            TotalIncome = "987.654"
        };

        MunsoftIndigent indigent = mapper.Map(row, instanceId: 42, mdbCode: "MDB-1");

        Assert.Equal(42, indigent.InstanceId);
        Assert.Equal("MDB-1", indigent.MDBCode);
        Assert.Equal("A-100", indigent.AccountNumber);
        Assert.Equal("SEQ-1", indigent.IndigentSequenceNumber);
        Assert.Equal(new DateOnly(2026, 10, 8), indigent.ApplicationDate);
        Assert.True(indigent.Approved);
        Assert.True(indigent.Active);
        Assert.Equal(7L, indigent.NumberOfOccupants);
        Assert.Equal(123.46m, indigent.IndigentIncome);
        Assert.Equal(987.65m, indigent.TotalIncome);
    }

    [Fact]
    public void TryMap_InvalidDate_CreatesFailureLog()
    {
        MunsoftIndigentMapper mapper = new MunsoftIndigentMapper();
        MunsoftIndigentRow row = new MunsoftIndigentRow
        {
            AccountNumber = "A-100",
            IndigentSequenceNumber = "SEQ-1",
            ApplicationDate = "invalid-date"
        };

        bool mapped = mapper.TryMap(row, rowNumber: 7, instanceId: 42, mdbCode: "MDB-1", out MunsoftIndigent? indigent, out MunsoftIndigentLog? log);

        Assert.False(mapped);
        Assert.Null(indigent);
        Assert.NotNull(log);
        Assert.Equal(42, log!.InstanceId);
        Assert.Equal("MDB-1", log.MDBCode);
        Assert.Equal("A-100", log.AccountNumber);
        Assert.Equal("SEQ-1", log.IndigentSequenceNumber);
        Assert.Equal(7, log.RowNumber);
        Assert.Contains("date", log.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
