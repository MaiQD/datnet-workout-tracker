using MediatR;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetSmartExerciseSuggestionsQueryHandler : IRequestHandler<GetSmartExerciseSuggestionsQuery, Result<IEnumerable<ExerciseDto>>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUserPreferencesProjectionRepository _preferencesRepository;

    public GetSmartExerciseSuggestionsQueryHandler(
        IExerciseRepository exerciseRepository,
        IUserPreferencesProjectionRepository preferencesRepository)
    {
        _exerciseRepository = exerciseRepository;
        _preferencesRepository = preferencesRepository;
    }

    public async Task<Result<IEnumerable<ExerciseDto>>> Handle(GetSmartExerciseSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var all = await _exerciseRepository.GetAllForUserAsync(request.UserId, cancellationToken);
        if (all.IsFailure) return Result.Failure<IEnumerable<ExerciseDto>>(all.Error!);

        var prefResult = await _preferencesRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var preferredMuscles = prefResult.Value?.FocusMuscleGroupIds ?? new List<string>();
        var availableEquipment = prefResult.Value?.AvailableEquipmentIds ?? new List<string>();

        var scored = all.Value!
            .Select(e => new
            {
                Exercise = e,
                Score = (e.MuscleGroups?.Count ?? 0) + (e.Equipment?.Count ?? 0)
                        + (e.MuscleGroups?.Count(m => preferredMuscles.Contains(m)) ?? 0) * 2
                        + (e.Equipment?.Count(eq => availableEquipment.Contains(eq)) ?? 0) * 2
            })
            .OrderByDescending(x => x.Score)
            .Take(request.Limit)
            .Select(x => ExerciseMapper.ToDto(x.Exercise));

        return Result.Success(scored);
    }
}


