using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(ApplicationUser.IsOnboarded))]
    [MapperIgnoreSource(nameof(ApplicationUser.OnboardingCompletedAt))]
    [MapperIgnoreSource(nameof(ApplicationUser.AvailableEquipmentIds))]
    [MapperIgnoreSource(nameof(ApplicationUser.FocusMuscleGroupIds))]
    [MapperIgnoreSource(nameof(ApplicationUser.AssignedPtId))]
    [MapperIgnoreSource(nameof(ApplicationUser.AssignedPt))]
    [MapperIgnoreSource(nameof(ApplicationUser.Clients))]
    public static partial UserDto ToDto(ApplicationUser user);
    public static partial IEnumerable<UserDto> ToDto(IEnumerable<ApplicationUser> users);
    
    // Custom mapping for enum conversions
    private static string MapLoginMethod(LoginMethod loginMethod) => loginMethod.ToString();
    private static string? MapGender(Gender? gender) => gender?.ToString();
    private static string MapUnitPreference(UnitPreference unitPreference) => unitPreference.ToString();
}
