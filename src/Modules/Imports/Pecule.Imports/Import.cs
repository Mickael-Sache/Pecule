using Pecule.Imports.Contracts;

namespace Pecule.Imports;

internal sealed class Import
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid AccountId { get; init; }
    public required string FileName { get; init; }
    public StatementFormat Format { get; init; }
    public DateTimeOffset ImportedAt { get; init; }
    public int LineCount { get; init; }
    public int ImportedCount { get; init; }
    public int DuplicateCount { get; init; }
    public int ErrorCount { get; init; }
}
