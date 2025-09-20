using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using dotFitness.Modules.Users.Infrastructure.Data.Configurations;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Users.Infrastructure.Data;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    // DbSets for domain entities
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserMetric> UserMetrics { get; set; } = null!;

    // DbSets for infrastructure entities
    public virtual DbSet<OutboxMessageEntity> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema for Users module
        modelBuilder.HasDefaultSchema("users");

        // Apply domain entity configurations
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserMetricEntityConfiguration());

        // Configure OutboxMessageEntity
        modelBuilder.Entity<OutboxMessageEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.EventId)
                .IsUnique()
                .HasDatabaseName("IX_OutboxMessages_EventId");

            entity.HasIndex(e => e.IsProcessed)
                .HasDatabaseName("IX_OutboxMessages_IsProcessed");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_OutboxMessages_CreatedAt");

            entity.HasIndex(e => e.EventType)
                .HasDatabaseName("IX_OutboxMessages_EventType");

            entity.HasIndex(e => new { e.IsProcessed, e.CreatedAt })
                .HasDatabaseName("IX_OutboxMessages_IsProcessed_CreatedAt");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update the UpdatedAt timestamp for modified entities
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is IEntity entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
