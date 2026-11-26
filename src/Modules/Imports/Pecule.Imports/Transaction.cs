namespace Pecule.Imports;

internal sealed class Transaction
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid AccountId { get; init; }
    public Guid ImportId { get; init; }
    public DateOnly Date { get; init; }
    public decimal Amount { get; init; }
    public required string RawLabel { get; init; }
    public required string NormalizedLabel { get; init; }
    public required string Fingerprint { get; init; }
    public Guid? CategoryId { get; set; }
}
