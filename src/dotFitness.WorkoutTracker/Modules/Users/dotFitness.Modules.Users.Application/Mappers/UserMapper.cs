using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.IsAdmin))]
    public partial UserDto ToDto(User user);
    
    // Custom mapping for enum conversions
    private string MapLoginMethod(LoginMethod loginMethod) => loginMethod.ToString();
    private string? MapGender(Gender? gender) => gender?.ToString();
    private string MapUnitPreference(UnitPreference unitPreference) => unitPreference.ToString();
}
