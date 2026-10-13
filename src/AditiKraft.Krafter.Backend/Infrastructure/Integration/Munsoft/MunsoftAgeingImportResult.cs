using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed record MunsoftAgeingImportResult(
    IReadOnlyList<MunsoftAgeing> Ageings,
    IReadOnlyList<MunsoftAgeingLog> Logs);
