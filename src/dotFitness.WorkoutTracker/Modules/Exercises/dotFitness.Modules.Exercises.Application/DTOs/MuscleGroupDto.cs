namespace dotFitness.Modules.Exercises.Application.DTOs;

public record MuscleGroupDto(
    string Id,
    string Name,
    string? Description,
    bool IsGlobal,
    string? UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
