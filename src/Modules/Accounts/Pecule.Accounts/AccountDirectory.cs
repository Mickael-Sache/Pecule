using Microsoft.EntityFrameworkCore;

using Pecule.Accounts.Contracts;

namespace Pecule.Accounts;

internal sealed class AccountDirectory(AccountsModule.AccountsDbContext db) : IAccountDirectory
{
    public Task<bool> IsOwnedByCurrentUserAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        db.Accounts.AnyAsync(a => a.Id == accountId, cancellationToken);
}
