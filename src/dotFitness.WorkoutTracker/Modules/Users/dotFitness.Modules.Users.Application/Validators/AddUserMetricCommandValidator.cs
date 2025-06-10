using FluentValidation;
using dotFitness.Modules.Users.Application.Commands;

namespace dotFitness.Modules.Users.Application.Validators;

public class AddUserMetricCommandValidator : AbstractValidator<AddUserMetricCommand>
{
    public AddUserMetricCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000)
            .When(x => x.Weight.HasValue)
            .WithMessage("Weight must be between 0 and 1000");

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .LessThanOrEqualTo(300)
            .When(x => x.Height.HasValue)
            .WithMessage("Height must be between 0 and 300");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x)
            .Must(x => x.Weight.HasValue || x.Height.HasValue)
            .WithMessage("At least one metric (weight or height) must be provided");
    }
}
