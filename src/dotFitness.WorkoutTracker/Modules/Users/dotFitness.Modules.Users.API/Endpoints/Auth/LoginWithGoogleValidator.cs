using dotFitness.Modules.Users.Application.DTOs;
using FastEndpoints;
using FluentValidation;

namespace dotFitness.Modules.Users.API.Endpoints.Auth;

/// <summary>
/// Validator for LoginWithGoogleRequest DTO
/// </summary>
public class LoginWithGoogleValidator : Validator<LoginWithGoogleRequest>
{
    public LoginWithGoogleValidator()
    {
        RuleFor(x => x.GoogleToken)
            .NotEmpty()
            .WithMessage("Google token ID is required");
    }
}

