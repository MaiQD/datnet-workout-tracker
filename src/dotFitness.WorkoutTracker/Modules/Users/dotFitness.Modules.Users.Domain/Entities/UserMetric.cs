using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Users.Domain.Entities;

public class UserMetric : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("date")]
    [BsonRequired]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date; // Store as date only

    [BsonElement("weight")]
    public double? Weight { get; set; } // in kg or lbs depending on user preference

    [BsonElement("height")]
    public double? Height { get; set; } // in cm or inches depending on user preference

    [BsonElement("bmi")]
    public double? Bmi { get; set; } // Calculated BMI

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void CalculateBmi(UnitPreference unitPreference)
    {
        if (!Weight.HasValue || !Height.HasValue || Weight <= 0 || Height <= 0)
        {
            Bmi = null;
            return;
        }

        double weightInKg = Weight.Value;
        double heightInMeters = Height.Value;

        // Convert to metric if needed
        if (unitPreference == UnitPreference.Imperial)
        {
            weightInKg = Weight.Value * 0.453592; // lbs to kg
            heightInMeters = Height.Value * 0.0254; // inches to meters
        }
        else
        {
            heightInMeters = Height.Value / 100; // cm to meters
        }

        Bmi = Math.Round(weightInKg / (heightInMeters * heightInMeters), 2);
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetBmiCategory()
    {
        if (!Bmi.HasValue) return "Unknown";

        return Bmi.Value switch
        {
            < 18.5 => "Underweight",
            >= 18.5 and < 25 => "Normal weight",
            >= 25 and < 30 => "Overweight",
            >= 30 => "Obese",
            _ => "Unknown"
        };
    }

    public void UpdateMetrics(double? weight = null, double? height = null, string? notes = null)
    {
        if (weight.HasValue && weight > 0)
            Weight = weight;

        if (height.HasValue && height > 0)
            Height = height;

        if (notes != null)
            Notes = notes;

        UpdatedAt = DateTime.UtcNow;
    }
}
