using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for User entity with dual-ID support
/// </summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("Users", "users");
        
        // Primary key - integer auto-increment
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("integer")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Google OAuth integration
        builder.Property(u => u.GoogleId)
            .HasColumnName("GoogleId")
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        builder.HasIndex(u => u.GoogleId)
            .HasDatabaseName("IX_Users_GoogleId")
            .IsUnique()
            .HasFilter("\"GoogleId\" IS NOT NULL");

        // Email configuration
        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(320)") // RFC 5321 max email length
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName("IX_Users_Email")
            .IsUnique();

        // Display name
        builder.Property(u => u.DisplayName)
            .HasColumnName("DisplayName")
            .HasColumnType("varchar(255)")
            .IsRequired();

        // Profile picture URL
        builder.Property(u => u.ProfilePicture)
            .HasColumnName("ProfilePicture")
            .HasColumnType("varchar(2048)")
            .IsRequired(false);

        // Login method enum
        builder.Property(u => u.LoginMethod)
            .HasColumnName("LoginMethod")
            .HasColumnType("varchar(50)")
            .HasConversion<string>()
            .IsRequired();

        // Roles as JSON array
        builder.Property(u => u.Roles)
            .HasColumnName("Roles")
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .IsRequired();

        // Gender enum
        builder.Property(u => u.Gender)
            .HasColumnName("Gender")
            .HasColumnType("varchar(50)")
            .HasConversion<string?>()
            .IsRequired(false);

        // Date of birth
        builder.Property(u => u.DateOfBirth)
            .HasColumnName("DateOfBirth")
            .HasColumnType("date")
            .IsRequired(false);

        // Unit preference enum
        builder.Property(u => u.UnitPreference)
            .HasColumnName("UnitPreference")
            .HasColumnType("varchar(50)")
            .HasConversion<string>()
            .IsRequired();

        // Timestamps
        builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        // Onboarding status
        builder.Property(u => u.IsOnboarded)
            .HasColumnName("IsOnboarded")
            .HasColumnType("boolean")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(u => u.OnboardingCompletedAt)
            .HasColumnName("OnboardingCompletedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        // Available equipment IDs as JSON array
        builder.Property(u => u.AvailableEquipmentIds)
            .HasColumnName("AvailableEquipmentIds")
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .IsRequired();

        // Focus muscle group IDs as JSON array
        builder.Property(u => u.FocusMuscleGroupIds)
            .HasColumnName("FocusMuscleGroupIds")
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        builder.HasIndex(u => u.IsOnboarded)
            .HasDatabaseName("IX_Users_IsOnboarded");

        builder.HasIndex(u => u.LoginMethod)
            .HasDatabaseName("IX_Users_LoginMethod");
    }
}
