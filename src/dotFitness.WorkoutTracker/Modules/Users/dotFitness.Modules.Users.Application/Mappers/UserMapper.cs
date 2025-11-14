using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public static partial class UserMapper
{
    // Ignore Identity properties that shouldn't be exposed in DTO
    [MapperIgnoreSource(nameof(ApplicationUser.UserName))]
    [MapperIgnoreSource(nameof(ApplicationUser.NormalizedUserName))]
    [MapperIgnoreSource(nameof(ApplicationUser.NormalizedEmail))]
    [MapperIgnoreSource(nameof(ApplicationUser.EmailConfirmed))]
    [MapperIgnoreSource(nameof(ApplicationUser.PasswordHash))]
    [MapperIgnoreSource(nameof(ApplicationUser.SecurityStamp))]
    [MapperIgnoreSource(nameof(ApplicationUser.ConcurrencyStamp))]
    [MapperIgnoreSource(nameof(ApplicationUser.PhoneNumber))]
    [MapperIgnoreSource(nameof(ApplicationUser.PhoneNumberConfirmed))]
    [MapperIgnoreSource(nameof(ApplicationUser.TwoFactorEnabled))]
    [MapperIgnoreSource(nameof(ApplicationUser.LockoutEnd))]
    [MapperIgnoreSource(nameof(ApplicationUser.LockoutEnabled))]
    [MapperIgnoreSource(nameof(ApplicationUser.AccessFailedCount))]
    // Ignore domain properties not needed in DTO
    [MapperIgnoreSource(nameof(ApplicationUser.IsOnboarded))]
    [MapperIgnoreSource(nameof(ApplicationUser.OnboardingCompletedAt))]
    [MapperIgnoreSource(nameof(ApplicationUser.AvailableEquipmentIds))]
    [MapperIgnoreSource(nameof(ApplicationUser.FocusMuscleGroupIds))]
    [MapperIgnoreSource(nameof(ApplicationUser.AssignedPtId))]
    [MapperIgnoreSource(nameof(ApplicationUser.AssignedPt))]
    [MapperIgnoreSource(nameof(ApplicationUser.Clients))]
    // Ignore target property that's populated separately (Roles come from Identity UserManager)
    [MapperIgnoreTarget(nameof(UserDto.Roles))]
    public static partial UserDto ToDto(ApplicationUser user);
    public static partial IEnumerable<UserDto> ToDto(IEnumerable<ApplicationUser> users);
    
    // Custom mapping for enum conversions
    private static string MapLoginMethod(LoginMethod loginMethod) => loginMethod.ToString();
    private static string? MapGender(Gender? gender) => gender?.ToString();
    private static string MapUnitPreference(UnitPreference unitPreference) => unitPreference.ToString();
}
