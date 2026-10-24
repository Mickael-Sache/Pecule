using Microsoft.EntityFrameworkCore;

namespace Pecule.Accounts;

public static partial class AccountsModule
{
    internal sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.HasDefaultSchema("accounts");
    }
}