using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Application.DTOs;

public class UpdateUserProfileRequest
{
    public UpdateUserProfileRequest()
    {
        
    }

    public UpdateUserProfileRequest(string displayName, Gender? gender, DateTime? dateOfBirth, UnitPreference? unitPreference)
    {
        DisplayName = displayName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        UnitPreference = unitPreference;
    }
    public string? DisplayName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UnitPreference? UnitPreference { get; set; }
}
