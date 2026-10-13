using AditiKraft.Krafter.Backend.Features.Integrations.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public sealed record MunsoftDebtorImportResult(
    IReadOnlyList<MunsoftDebtor> Debtors,
    IReadOnlyList<MunsoftDebtorLog> Logs);
