using MediatR;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetSmartExerciseSuggestionsQueryHandler : IRequestHandler<GetSmartExerciseSuggestionsQuery, Result<IEnumerable<ExerciseDto>>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ExerciseMapper _mapper;

    public GetSmartExerciseSuggestionsQueryHandler(
        IExerciseRepository exerciseRepository,
        IUserRepository userRepository,
        ExerciseMapper mapper)
    {
        _exerciseRepository = exerciseRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ExerciseDto>>> Handle(GetSmartExerciseSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
            return Result.Success(Enumerable.Empty<ExerciseDto>());

        var user = userResult.Value;
        var preferredMuscles = user.FocusMuscleGroupIds ?? new List<string>();
        var availableEquipment = user.AvailableEquipmentIds ?? new List<string>();

        var all = await _exerciseRepository.GetAllForUserAsync(request.UserId, cancellationToken);
        if (all.IsFailure) return Result.Failure<IEnumerable<ExerciseDto>>(all.Error!);

        var scored = all.Value
            .Select(e => new
            {
                Exercise = e,
                Score = (e.MuscleGroups?.Count ?? 0) + (e.Equipment?.Count ?? 0)
            })
            .OrderByDescending(x => x.Score)
            .Take(request.Limit)
            .Select(x => _mapper.ToDto(x.Exercise));

        return Result.Success(scored);
    }
}


