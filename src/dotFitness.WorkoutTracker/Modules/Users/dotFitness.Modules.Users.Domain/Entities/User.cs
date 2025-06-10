using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Users.Domain.Entities;

public class User : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("googleId")]
    public string? GoogleId { get; set; }

    [BsonElement("email")]
    [BsonRequired]
    public string Email { get; set; } = string.Empty;

    [BsonElement("displayName")]
    [BsonRequired]
    public string DisplayName { get; set; } = string.Empty;

    [BsonElement("loginMethod")]
    [BsonRepresentation(BsonType.String)]
    public LoginMethod LoginMethod { get; set; } = LoginMethod.Google;

    [BsonElement("roles")]
    public List<string> Roles { get; set; } = new() { "User" };

    [BsonElement("gender")]
    [BsonRepresentation(BsonType.String)]
    public Gender? Gender { get; set; }

    [BsonElement("dateOfBirth")]
    public DateTime? DateOfBirth { get; set; }

    [BsonElement("unitPreference")]
    [BsonRepresentation(BsonType.String)]
    public UnitPreference UnitPreference { get; set; } = UnitPreference.Metric;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

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
        if (!string.IsNullOrWhiteSpace(displayName))
            DisplayName = displayName;

        if (gender.HasValue)
            Gender = gender;

        if (dateOfBirth.HasValue)
            DateOfBirth = dateOfBirth;

        if (unitPreference.HasValue)
            UnitPreference = unitPreference.Value;

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
    Metric,    // kg, cm
    Imperial   // lbs, inches
}
