namespace Pecule.Accounts;

public static partial class AccountsModule
{
    internal enum AccountType
    {
        Courant,
        Livret
    }

    internal sealed class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public AccountType Type { get; set; }
        public decimal InitialBalance { get; set; }
        public string Devise { get; set; } = "EUR"; // Default currency
    }
}