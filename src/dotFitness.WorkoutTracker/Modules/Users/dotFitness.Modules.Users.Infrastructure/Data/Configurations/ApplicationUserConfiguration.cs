using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Table name (Identity tables use AspNetUsers by default)
        builder.ToTable("AspNetUsers");
        
        // Custom properties
        builder.Property(e => e.DisplayName)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(e => e.GoogleId)
            .HasMaxLength(100);
        
        builder.Property(e => e.ProfilePicture)
            .HasMaxLength(500);
        
        builder.Property(e => e.LoginMethod)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.Gender)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.DateOfBirth)
            .HasColumnType("date");
        
        builder.Property(e => e.UnitPreference)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        // Indexes
        builder.HasIndex(e => e.GoogleId)
            .IsUnique()
            .HasFilter("\"GoogleId\" IS NOT NULL");
        
        builder.HasIndex(e => e.Email)
            .IsUnique();
        
        // Collections stored as PostgreSQL native arrays (more efficient than JSONB for simple lists)
        builder.Property(e => e.AvailableEquipmentIds)
            .HasColumnType("text[]");
        
        builder.Property(e => e.FocusMuscleGroupIds)
            .HasColumnType("text[]");
        
        // PT relationship configuration
        builder.HasOne(e => e.AssignedPt)
            .WithMany(e => e.Clients)
            .HasForeignKey(e => e.AssignedPtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
