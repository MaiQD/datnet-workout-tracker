using FluentValidation;
using dotFitness.Modules.Users.Application.Commands;

namespace dotFitness.Modules.Users.Application.Validators;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required")
            .Length(1, 100)
            .WithMessage("Display name must be between 1 and 100 characters");

        RuleFor(x => x.Gender)
            .Must(gender => gender == null || new[] { "Male", "Female", "Other", "PreferNotToSay" }.Contains(gender))
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob == null || dob <= DateTime.UtcNow)
            .WithMessage("Date of birth cannot be in the future")
            .Must(dob => dob == null || dob >= DateTime.UtcNow.AddYears(-150))
            .WithMessage("Date of birth cannot be more than 150 years ago")
            .Must(dob => dob == null || dob <= DateTime.UtcNow.AddYears(-13))
            .WithMessage("User must be at least 13 years old");

        RuleFor(x => x.UnitPreference)
            .Must(unit => string.IsNullOrWhiteSpace(unit) || new[] { "Metric", "Imperial" }.Contains(unit))
            .WithMessage("Unit preference must be Metric or Imperial");
    }
}