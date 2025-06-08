using MongoDB.Bson.Serialization.Attributes;
using DatNetWorkoutTracker.Shared.Domain;

namespace DatNetWorkoutTracker.Users.Domain;

public class User : BaseEntity
{
    [BsonElement("googleId")]
    public string GoogleId { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("profilePictureUrl")]
    public string? ProfilePictureUrl { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("lastLoginAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastLoginAt { get; set; }

    [BsonElement("preferences")]
    public UserPreferences Preferences { get; set; } = new();
}

public class UserPreferences
{
    [BsonElement("preferredWeightUnit")]
    public WeightUnit PreferredWeightUnit { get; set; } = WeightUnit.Kilograms;

    [BsonElement("preferredDistanceUnit")]
    public DistanceUnit PreferredDistanceUnit { get; set; } = DistanceUnit.Kilometers;

    [BsonElement("theme")]
    public string Theme { get; set; } = "light";
}

public enum WeightUnit
{
    Kilograms,
    Pounds
}

public enum DistanceUnit
{
    Kilometers,
    Miles
}
