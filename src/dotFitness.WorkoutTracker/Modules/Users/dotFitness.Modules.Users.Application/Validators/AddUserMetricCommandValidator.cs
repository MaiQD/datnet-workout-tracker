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
            .Must(weight => !weight.HasValue || (weight > 0 && weight <= 1000))
            .WithMessage("Weight must be between 0 and 1000");

        RuleFor(x => x.Height)
            .Must(height => !height.HasValue || (height > 0 && height <= 300))
            .WithMessage("Height must be between 0 and 300");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x)
            .Must(x => x.Weight.HasValue || x.Height.HasValue)
            .WithMessage("At least one metric (weight or height) must be provided");
    }
}
