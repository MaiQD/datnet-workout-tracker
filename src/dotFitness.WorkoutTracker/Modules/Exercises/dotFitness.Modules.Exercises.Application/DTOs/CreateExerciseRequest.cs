using dotFitness.Modules.Exercises.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Exercises.Application.DTOs;

public record CreateExerciseRequest(
    [Required, StringLength(100, MinimumLength = 1)]
    string Name,
    
    [StringLength(500)]
    string? Description,
    
    List<string> MuscleGroups,
    List<string> Equipment,
    List<string> Instructions,
    
    ExerciseDifficulty Difficulty = ExerciseDifficulty.Beginner,
    
    [Url]
    string? VideoUrl = null,
    
    [Url] 
    string? ImageUrl = null,
    
    List<string> Tags = default!
)
{
    public List<string> Tags { get; init; } = Tags ?? new List<string>();
};
