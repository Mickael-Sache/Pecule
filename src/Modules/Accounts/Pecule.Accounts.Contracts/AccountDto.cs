namespace Pecule.Accounts.Contracts;

public sealed record AccountDto(Guid Id, string Name, AccountType Type, decimal InitialBalance, string Currency);
