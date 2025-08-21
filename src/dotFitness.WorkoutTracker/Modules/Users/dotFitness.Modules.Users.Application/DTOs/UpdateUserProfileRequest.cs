using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Application.DTOs;

public class UpdateUserProfileRequest
{
    public string? DisplayName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UnitPreference? UnitPreference { get; set; }
}
