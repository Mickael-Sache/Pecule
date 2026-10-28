using Pecule.Accounts.Contracts;

namespace Pecule.Accounts;

internal sealed class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
    public string Currency { get; set; } = "EUR"; // Default currency
}
