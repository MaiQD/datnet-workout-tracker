using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using FastEndpoints;
using FluentValidation;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Validator for UpdateUserProfileRequest
/// </summary>
public class UpdateUserProfileValidator : Validator<UpdateUserProfileRequest>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(100)
            .WithMessage("Display name must be 100 characters or less.")
            .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob == null || dob <= DateTime.UtcNow)
            .WithMessage("Date of birth cannot be in the future")
            .Must(dob => dob == null || dob >= DateTime.UtcNow.AddYears(-150))
            .WithMessage("Date of birth cannot be more than 150 years ago")
            .Must(dob => dob == null || dob <= DateTime.UtcNow.AddYears(-13))
            .WithMessage("User must be at least 13 years old");
    }
}

