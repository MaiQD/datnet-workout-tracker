using dotFitness.Modules.Users.Application.Commands;
using FluentValidation;

namespace dotFitness.Modules.Users.Application.Validators;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Request.DisplayName)
            .MaximumLength(100)
            .WithMessage("Display name must be 100 characters or less.")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.DisplayName));

        RuleFor(x => x.Request.DateOfBirth)
            .Must(dob => dob == null || dob <= DateTime.UtcNow)
            .WithMessage("Date of birth cannot be in the future")
            .Must(dob => dob == null || dob >= DateTime.UtcNow.AddYears(-150))
            .WithMessage("Date of birth cannot be more than 150 years ago")
            .Must(dob => dob == null || dob <= DateTime.UtcNow.AddYears(-13))
            .WithMessage("User must be at least 13 years old");
    }
}