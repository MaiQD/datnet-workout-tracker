namespace dotFitness.Modules.Users.Domain.Events;

public record UserProfileUpdatedEvent(
    string UserId,
    string DisplayName,
    string? Gender,
    DateTime? DateOfBirth,
    string UnitPreference,
    DateTime UpdatedAt
);
