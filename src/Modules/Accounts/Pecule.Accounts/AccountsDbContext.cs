using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Pecule.Accounts;

public static partial class AccountsModule
{
    internal sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options, CurrentUser currentUser) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Account> Accounts => Set<Account>();

        // Expose the current user's ID for use in the DbContext
        private Guid? CurrentUserId => currentUser.Id;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("accounts");

            // Configure the relationship between Account and User
            modelBuilder.Entity<Account>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Apply a global query filter to ensure that only accounts belonging to the current user are returned
            modelBuilder.Entity<Account>().HasQueryFilter(a => a.UserId == CurrentUserId);
        }
    }
}
