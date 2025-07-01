using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, Result<IEnumerable<EquipmentDto>>>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ILogger<GetAllEquipmentQueryHandler> _logger;

    public GetAllEquipmentQueryHandler(
        IEquipmentRepository equipmentRepository,
        ILogger<GetAllEquipmentQueryHandler> logger)
    {
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<EquipmentDto>>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all equipment for user {UserId}", request.UserId);

            var result = await _equipmentRepository.GetAllForUserAsync(request.UserId, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get equipment for user {UserId}: {Error}", request.UserId, result.Error);
                return Result.Failure<IEnumerable<EquipmentDto>>(result.Error!);
            }

            var equipmentDtos = EquipmentMapper.ToDto(result.Value!);
            _logger.LogInformation("Successfully retrieved {Count} equipment for user {UserId}", 
                equipmentDtos.Count(), request.UserId);
            
            return Result.Success(equipmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting equipment for user {UserId}: {ErrorMessage}", 
                request.UserId, ex.Message);
            return Result.Failure<IEnumerable<EquipmentDto>>($"Failed to get equipment: {ex.Message}");
        }
    }
}
