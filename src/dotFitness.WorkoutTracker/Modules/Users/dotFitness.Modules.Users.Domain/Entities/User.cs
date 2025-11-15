using Microsoft.AspNetCore.Identity;

namespace dotFitness.Modules.Users.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? GoogleId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public LoginMethod LoginMethod { get; set; } = LoginMethod.Google;
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UnitPreference UnitPreference { get; set; } = UnitPreference.Metric;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsOnboarded { get; set; }
    public DateTime? OnboardingCompletedAt { get; set; }
    public List<string> AvailableEquipmentIds { get; set; } = [];
    public List<string> FocusMuscleGroupIds { get; set; } = [];
    
    // PT relationship
    public Guid? AssignedPtId { get; set; }
    public ApplicationUser? AssignedPt { get; set; }
    
    // Clients for PT users
    public ICollection<ApplicationUser> Clients { get; set; } = new List<ApplicationUser>();
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