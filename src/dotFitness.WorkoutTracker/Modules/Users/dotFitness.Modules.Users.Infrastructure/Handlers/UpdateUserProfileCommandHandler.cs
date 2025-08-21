using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Events;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.SharedKernel.Outbox;
using dotFitness.SharedKernel.Results;
using MediatR;
using MongoDB.Driver;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;
    private readonly IMongoCollection<OutboxMessage> _outboxCollection;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        UserMapper userMapper,
        IMongoCollection<OutboxMessage> outboxCollection)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
        _outboxCollection = outboxCollection;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (!userResult.IsSuccess)
        {
            return Result.Failure<UserDto>("User not found.");
        }

        var user = userResult.Value!;

        // Store original values for change detection
        var originalDisplayName = user.DisplayName;
        var originalGender = user.Gender;
        var originalDateOfBirth = user.DateOfBirth;
        var originalUnitPreference = user.UnitPreference;

        // Update the user profile using the domain method
        user.UpdateProfile(
            request.Request.DisplayName,
            request.Request.Gender,
            request.Request.DateOfBirth,
            request.Request.UnitPreference);

        // Check if any changes were made
        if (user.DisplayName == originalDisplayName &&
            user.Gender == originalGender &&
            user.DateOfBirth == originalDateOfBirth &&
            user.UnitPreference == originalUnitPreference)
        {
            // No changes made, return current user
            return Result.Success(_userMapper.ToDto(user));
        }

        // Save the updated user
        var updateResult = await _userRepository.UpdateAsync(user, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result.Failure<UserDto>("Failed to update user profile.");
        }

        var updatedUser = updateResult.Value!;

        // Create and save the domain event
        var profileUpdatedEvent = new UserProfileUpdatedEvent(
            user.Id,
            user.DisplayName,
            user.Gender?.ToString(),
            user.DateOfBirth,
            user.UnitPreference.ToString(),
            user.UpdatedAt);

        var outboxMessage = OutboxMessage.Create(profileUpdatedEvent);
        await _outboxCollection.InsertOneAsync(outboxMessage, cancellationToken: cancellationToken);

        return Result.Success(_userMapper.ToDto(updatedUser));
    }
}