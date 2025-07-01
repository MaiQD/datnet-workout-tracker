using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetAllMuscleGroupsQueryHandler : IRequestHandler<GetAllMuscleGroupsQuery, Result<IEnumerable<MuscleGroupDto>>>
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly ILogger<GetAllMuscleGroupsQueryHandler> _logger;

    public GetAllMuscleGroupsQueryHandler(
        IMuscleGroupRepository muscleGroupRepository,
        ILogger<GetAllMuscleGroupsQueryHandler> logger)
    {
        _muscleGroupRepository = muscleGroupRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<MuscleGroupDto>>> Handle(GetAllMuscleGroupsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all muscle groups for user {UserId}", request.UserId);

            var result = await _muscleGroupRepository.GetAllForUserAsync(request.UserId, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get muscle groups for user {UserId}: {Error}", request.UserId, result.Error);
                return Result.Failure<IEnumerable<MuscleGroupDto>>(result.Error!);
            }

            var muscleGroupDtos = MuscleGroupMapper.ToDto(result.Value!);
            _logger.LogInformation("Successfully retrieved {Count} muscle groups for user {UserId}", 
                muscleGroupDtos.Count(), request.UserId);
            
            return Result.Success(muscleGroupDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting muscle groups for user {UserId}: {ErrorMessage}", 
                request.UserId, ex.Message);
            return Result.Failure<IEnumerable<MuscleGroupDto>>($"Failed to get muscle groups: {ex.Message}");
        }
    }
}
