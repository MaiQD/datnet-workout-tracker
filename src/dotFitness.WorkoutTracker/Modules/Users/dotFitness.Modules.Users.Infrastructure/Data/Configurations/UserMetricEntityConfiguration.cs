using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for UserMetric entity
/// </summary>
public class UserMetricEntityConfiguration : IEntityTypeConfiguration<UserMetric>
{
    public void Configure(EntityTypeBuilder<UserMetric> builder)
    {
        // Table configuration
        builder.ToTable("UserMetrics", "users");
        
        // Primary key - integer auto-increment
        builder.HasKey(um => um.Id);
        builder.Property(um => um.Id)
            .HasColumnName("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // User ID foreign key
        builder.Property(um => um.UserId)
            .HasColumnName("UserId")
            .HasColumnType("integer")
            .IsRequired();

        // Create index on UserId for performance
        builder.HasIndex(um => um.UserId)
            .HasDatabaseName("IX_UserMetrics_UserId");

        // Date configuration
        builder.Property(um => um.Date)
            .HasColumnName("Date")
            .HasColumnType("date")
            .IsRequired();

        // Unique constraint on UserId + Date (one metric per user per day)
        builder.HasIndex(um => new { um.UserId, um.Date })
            .HasDatabaseName("IX_UserMetrics_UserId_Date")
            .IsUnique();

        // Weight in kilograms (always stored in metric)
        builder.Property(um => um.Weight)
            .HasColumnName("Weight")
            .HasColumnType("decimal(5,2)") // Up to 999.99 kg
            .IsRequired(false);

        // Height in centimeters (always stored in metric)
        builder.Property(um => um.Height)
            .HasColumnName("Height")
            .HasColumnType("decimal(5,2)") // Up to 999.99 cm
            .IsRequired(false);

        // BMI calculation result
        builder.Property(um => um.Bmi)
            .HasColumnName("Bmi")
            .HasColumnType("decimal(4,2)") // Up to 99.99 BMI
            .IsRequired(false);

        // Notes
        builder.Property(um => um.Notes)
            .HasColumnName("Notes")
            .HasColumnType("text")
            .IsRequired(false);

        // Timestamps
        builder.Property(um => um.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(um => um.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // Additional indexes for performance
        builder.HasIndex(um => um.Date)
            .HasDatabaseName("IX_UserMetrics_Date");

        builder.HasIndex(um => um.CreatedAt)
            .HasDatabaseName("IX_UserMetrics_CreatedAt");

        // Add computed column for BMI category if needed
        // This could be useful for analytics
        builder.HasIndex(um => um.Bmi)
            .HasDatabaseName("IX_UserMetrics_Bmi")
            .HasFilter("\"Bmi\" IS NOT NULL");
    }
}
