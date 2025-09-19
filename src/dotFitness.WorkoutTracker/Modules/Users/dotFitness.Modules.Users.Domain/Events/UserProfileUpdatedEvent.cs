namespace dotFitness.Modules.Users.Domain.Events;

public record UserProfileUpdatedEvent(
    int UserId,
    string DisplayName,
    string? Gender,
    DateTime? DateOfBirth,
    string UnitPreference,
    DateTime UpdatedAt
);
