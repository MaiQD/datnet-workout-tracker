using dotFitness.Modules.Exercises.Application.Commands;
using FluentValidation;

namespace dotFitness.Modules.Exercises.Application.Validators;

public class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Exercise name is required")
            .Length(1, 100)
            .WithMessage("Exercise name must be between 1 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.MuscleGroups)
            .NotEmpty()
            .WithMessage("At least one muscle group is required")
            .Must(mg => mg.All(m => !string.IsNullOrWhiteSpace(m)))
            .WithMessage("Muscle group names cannot be empty");

        RuleFor(x => x.Equipment)
            .NotNull()
            .WithMessage("Equipment list is required")
            .Must(eq => eq.All(e => !string.IsNullOrWhiteSpace(e)))
            .WithMessage("Equipment names cannot be empty");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("At least one instruction is required")
            .Must(inst => inst.All(i => !string.IsNullOrWhiteSpace(i)))
            .WithMessage("Instructions cannot be empty");

        RuleFor(x => x.VideoUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.VideoUrl))
            .WithMessage("Video URL must be a valid URL");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Image URL must be a valid URL");

        RuleFor(x => x.Tags)
            .NotNull()
            .WithMessage("Tags list is required")
            .Must(tags => tags.All(t => !string.IsNullOrWhiteSpace(t)))
            .WithMessage("Tag names cannot be empty");
    }

    private static bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
