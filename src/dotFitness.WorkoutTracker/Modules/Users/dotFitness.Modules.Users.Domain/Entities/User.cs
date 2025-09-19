using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Users.Domain.Entities;

public class User : IEntity
{
    // Primary key for all databases
    public int Id { get; set; }

    public string? GoogleId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public LoginMethod LoginMethod { get; set; } = LoginMethod.Google;
    public List<string> Roles { get; set; } = new() { "User" };
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UnitPreference UnitPreference { get; set; } = UnitPreference.Metric;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsOnboarded { get; set; } = false;
    public DateTime? OnboardingCompletedAt { get; set; }
    public List<int> AvailableEquipmentIds { get; set; } = new();
    public List<int> FocusMuscleGroupIds { get; set; } = new();

    public bool IsAdmin => Roles.Contains("Admin");

    public void AddRole(string role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveRole(string role)
    {
        if (Roles.Contains(role) && role != "User") // Prevent removing base User role
        {
            Roles.Remove(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateProfile(string? displayName = null, Gender? gender = null,
        DateTime? dateOfBirth = null, UnitPreference? unitPreference = null)
    {
        var isUpdated = false;
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            isUpdated = true;
            DisplayName = displayName;
        }

        if (gender.HasValue)
        {
            isUpdated = true;
            Gender = gender;
        }

        if (dateOfBirth.HasValue)
        {
            isUpdated = true;
            DateOfBirth = dateOfBirth;
        }

        if (unitPreference.HasValue)
        {
            isUpdated = true;
            UnitPreference = unitPreference.Value;
        }

        if (isUpdated)
            UpdatedAt = DateTime.UtcNow;
    }
}

public enum LoginMethod
{
    Google,
    Microsoft,
    Apple
}

public enum Gender
{
    Male,
    Female,
    Other,
    PreferNotToSay
}

public enum UnitPreference
{
    Metric, // kg, cm
    Imperial // lbs, inches
}