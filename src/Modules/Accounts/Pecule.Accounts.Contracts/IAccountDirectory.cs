namespace Pecule.Accounts.Contracts;

public interface IAccountDirectory
{
    Task<bool> IsOwnedByCurrentUserAsync(Guid accountId, CancellationToken cancellationToken = default);
}
