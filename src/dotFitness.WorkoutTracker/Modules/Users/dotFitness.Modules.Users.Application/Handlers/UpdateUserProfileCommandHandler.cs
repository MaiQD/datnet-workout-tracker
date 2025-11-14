using dotFitness.Common.Events;
using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Handlers;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IOutboxPublisher outboxPublisher,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _outboxPublisher = outboxPublisher;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // 1. Get user from repository
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<UserDto>(userResult.Error ?? "User not found.");
        }

        var user = userResult.Value!;

        // Store original values for change detection
        var originalDisplayName = user.DisplayName;
        var originalGender = user.Gender;
        var originalDateOfBirth = user.DateOfBirth;
        var originalUnitPreference = user.UnitPreference;

        // 2. Update the user profile properties directly
        if (!string.IsNullOrWhiteSpace(request.Request.DisplayName))
            user.DisplayName = request.Request.DisplayName;
        
        if (request.Request.Gender.HasValue)
            user.Gender = request.Request.Gender.Value;
        
        if (request.Request.DateOfBirth.HasValue)
            user.DateOfBirth = request.Request.DateOfBirth.Value;
        
        if (request.Request.UnitPreference.HasValue)
            user.UnitPreference = request.Request.UnitPreference.Value;
        
        user.UpdatedAt = DateTime.UtcNow;

        // Check if any changes were made
        if (user.DisplayName == originalDisplayName &&
            user.Gender == originalGender &&
            user.DateOfBirth == originalDateOfBirth &&
            user.UnitPreference == originalUnitPreference)
        {
            // No changes made, return current user
            return Result.Success(UserMapper.ToDto(user));
        }

        // 3. Save changes via repository
        var updateResult = await _userRepository.UpdateAsync(user, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UserDto>(updateResult.Error ?? "Failed to update user profile.");
        }

        // 4. Publish domain event to outbox
        var profileUpdatedEvent = new UserProfileUpdatedEvent(
            user.Id,
            user.DisplayName,
            user.Gender?.ToString(),
            user.DateOfBirth,
            user.UnitPreference.ToString(),
            user.UpdatedAt);

        await _outboxPublisher.PublishAsync(profileUpdatedEvent, cancellationToken);
        
        _logger.LogInformation("Successfully updated profile for user {UserId}", user.Id);
        return Result.Success(UserMapper.ToDto(user));
    }
}

