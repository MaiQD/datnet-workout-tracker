using FastEndpoints;
using FluentValidation;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Validator for GetUserMetricsRequest
/// </summary>
public class GetUserMetricsValidator : Validator<GetUserMetricsRequest>
{
    public GetUserMetricsValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Take cannot exceed 100");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}

