using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using dotFitness.Modules.Users.Infrastructure.Data.Configurations;

namespace dotFitness.Modules.Users.Infrastructure.Data;

public class UsersDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) 
    {
    }
    
    public virtual DbSet<UserMetric> UserMetrics { get; set; } = null!;
    public virtual DbSet<OutboxMessageEntity> OutboxMessages { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Critical: Call base first for Identity
        
        // Set default schema
        modelBuilder.HasDefaultSchema("users");
        
        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserMetricEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for ApplicationUser
        var entries = ChangeTracker.Entries<ApplicationUser>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
