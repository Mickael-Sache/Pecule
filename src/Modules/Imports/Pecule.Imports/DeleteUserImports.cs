using Microsoft.EntityFrameworkCore;

using Pecule.Accounts.Contracts;
using Pecule.SharedKernel;

namespace Pecule.Imports;

internal sealed class DeleteUserImports(ImportsDbContext db) : IEventSubscriber<UserDeleted>
{
    public async Task HandleAsync(UserDeleted message, CancellationToken cancellationToken)
    {
        // Without global filter : we aim on deleted user
        await db.Transactions.IgnoreQueryFilters().Where(t => t.UserId == message.UserId).ExecuteDeleteAsync(cancellationToken);
        await db.Imports.IgnoreQueryFilters().Where(i => i.UserId == message.UserId).ExecuteDeleteAsync(cancellationToken);
    }
}
