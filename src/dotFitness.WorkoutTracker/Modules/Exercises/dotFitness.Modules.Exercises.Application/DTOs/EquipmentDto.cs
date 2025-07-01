namespace dotFitness.Modules.Exercises.Application.DTOs;

public record EquipmentDto(
    string Id,
    string Name,
    string? Description,
    string? Category,
    bool IsGlobal,
    string? UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
