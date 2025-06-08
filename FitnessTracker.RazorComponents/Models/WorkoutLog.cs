using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace FitnessTracker.RazorComponents.Models
{
    public class WorkoutLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        public string UserId { get; set; } = string.Empty;
        
        public DateTime Date { get; set; } = DateTime.Today;
        
        public string Title { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        public int DurationMinutes { get; set; } = 30;
        
        public List<WorkoutExerciseEntry> ExerciseEntries { get; set; } = new List<WorkoutExerciseEntry>();
    }
    
    public class WorkoutExerciseEntry
    {
        public string ExerciseId { get; set; } = string.Empty;
        
        public int Sets { get; set; } = 3;
        
        public int Reps { get; set; } = 10;
        
        public double Weight { get; set; } = 0;
        
        public string Notes { get; set; } = string.Empty;
    }
}
