using Microsoft.EntityFrameworkCore;

using Pecule.SharedKernel;

namespace Pecule.Imports;

internal sealed class ImportsDbContext(DbContextOptions<ImportsDbContext> options, ICurrentUser currentUser) : DbContext(options)
{
    public DbSet<Import> Imports => Set<Import>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    private readonly Guid? _currentUserId = currentUser.Id;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("imports");

        modelBuilder.Entity<Import>(import =>
        {
            import.Property(i => i.FileName).HasMaxLength(256);
            import.Property(i => i.Format).HasConversion<string>().HasMaxLength(20);
            import.HasIndex(i => new { i.UserId, i.ImportedAt });
            import.HasQueryFilter(i => i.UserId == _currentUserId);
        });

        modelBuilder.Entity<Transaction>(transaction =>
        {
            transaction.Property(t => t.Amount).HasPrecision(18, 2);
            transaction.Property(t => t.RawLabel).HasMaxLength(500);
            transaction.Property(t => t.NormalizedLabel).HasMaxLength(500);
            transaction.Property(t => t.Fingerprint).HasMaxLength(64).IsFixedLength();
            transaction.HasIndex(t => new { t.AccountId, t.Date });
            transaction.HasIndex(t => new { t.AccountId, t.Fingerprint }).IsUnique();
            transaction.HasIndex(t => t.UserId);

            // Cancel one Import delete all associated operations
            transaction.HasOne<Import>().WithMany().HasForeignKey(t => t.ImportId).OnDelete(DeleteBehavior.Cascade);
            transaction.HasQueryFilter(t => t.UserId == _currentUserId);
        });
    }
}
