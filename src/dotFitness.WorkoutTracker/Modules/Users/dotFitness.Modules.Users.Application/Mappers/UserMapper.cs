using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.IsAdmin))]
    [MapperIgnoreSource(nameof(User.IsOnboarded))]
    [MapperIgnoreSource(nameof(User.OnboardingCompletedAt))]
    [MapperIgnoreSource(nameof(User.AvailableEquipmentIds))]
    [MapperIgnoreSource(nameof(User.FocusMuscleGroupIds))]
    public static partial UserDto ToDto(User user);
    public static partial IEnumerable<UserDto> ToDto(IEnumerable<User> users);
    
    // Custom mapping for enum conversions
    private static string MapLoginMethod(LoginMethod loginMethod) => loginMethod.ToString();
    private static string? MapGender(Gender? gender) => gender?.ToString();
    private static string MapUnitPreference(UnitPreference unitPreference) => unitPreference.ToString();
}
