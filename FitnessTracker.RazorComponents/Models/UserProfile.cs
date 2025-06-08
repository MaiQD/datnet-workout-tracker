using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessTracker.RazorComponents.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        public string UserId { get; set; } = string.Empty; // Authentication ID
        
        public string Name { get; set; } = string.Empty;
        
        public DateTime DateOfBirth { get; set; }
        
        public string Gender { get; set; } = string.Empty;
        
        public double HeightInCm { get; set; }
        
        public List<WeightEntry> WeightHistory { get; set; } = new List<WeightEntry>();
        
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();
    }
    
    public class WeightEntry
    {
        public DateTime Date { get; set; }
        
        public double WeightInKg { get; set; }
    }
}
