using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Data.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessageEntity>
{
    public void Configure(EntityTypeBuilder<OutboxMessageEntity> builder)
    {
        builder.ToTable("OutboxMessages");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.EventId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.EventData)
            .IsRequired()
            .HasColumnType("jsonb");
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Indexes
        builder.HasIndex(e => e.EventId)
            .IsUnique()
            .HasDatabaseName("IX_OutboxMessages_EventId");
        
        builder.HasIndex(e => e.IsProcessed)
            .HasDatabaseName("IX_OutboxMessages_IsProcessed");
        
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_OutboxMessages_CreatedAt");
        
        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_OutboxMessages_EventType");
        
        builder.HasIndex(e => new { e.IsProcessed, e.CreatedAt })
            .HasDatabaseName("IX_OutboxMessages_IsProcessed_CreatedAt");
    }
}
