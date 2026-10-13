using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed record MunsoftIndigentImportResult(
    IReadOnlyList<MunsoftIndigent> Indigents,
    IReadOnlyList<MunsoftIndigentLog> Logs);
