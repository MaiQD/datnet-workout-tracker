using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Commands;

public class UpdateUserProfileCommand(
    string userId,
    string displayName,
    string? gender,
    DateTime? dateOfBirth,
    string unitPreference = "Metric"
) : IRequest<Result<UserDto>>
{
    [Required] public string UserId { get; set; } = userId;

    [Required, StringLength(100, MinimumLength = 1)]
    public string DisplayName { get; init; } = displayName;

    [StringLength(10)] public string? Gender { get; init; } = gender;
    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; init; } = dateOfBirth;
    [Required, StringLength(20)] public string UnitPreference { get; init; } = unitPreference;
}