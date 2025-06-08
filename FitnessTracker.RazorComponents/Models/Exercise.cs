using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace FitnessTracker.RazorComponents.Models
{
    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string Category { get; set; } = string.Empty; // e.g., "Strength", "Cardio", "Flexibility"
        
        public string MuscleGroup { get; set; } = string.Empty; // e.g., "Chest", "Back", "Legs"
        
        public bool IsCustom { get; set; } = false; // Indicates if this is a user-created exercise
        
        public string YoutubeLink { get; set; } = string.Empty; // YouTube video link demonstrating the exercise
        
        public List<string> RequiredEquipment { get; set; } = new List<string>(); // List of equipment needed for the exercise
    }
}
