using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Application.DTOs;

public record ExerciseDto(
    string Id,
    string Name,
    string? Description,
    List<string> MuscleGroups,
    List<string> Equipment,
    List<string> Instructions,
    ExerciseDifficulty Difficulty,
    string? VideoUrl,
    string? ImageUrl,
    bool IsGlobal,
    string? UserId,
    List<string> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
