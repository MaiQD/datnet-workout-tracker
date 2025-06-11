using FluentValidation;
using dotFitness.Modules.Users.Application.Commands;

namespace dotFitness.Modules.Users.Application.Validators;

public class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleCommandValidator()
    {
        RuleFor(x => x.GoogleToken)
            .NotEmpty()
            .WithMessage("Google token ID is required");
    }
}
