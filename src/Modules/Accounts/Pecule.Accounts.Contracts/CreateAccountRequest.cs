namespace Pecule.Accounts.Contracts;

public sealed record CreateAccountRequest(string Name, AccountType Type, decimal InitialBalance);
