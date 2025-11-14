using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using dotFitness.Common.Events;
using dotFitness.Common.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly UsersDbContext _context;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        UsersDbContext context,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Use execution strategy to handle retries and transactions together
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                
                try
                {
                    // 1. Get user from database
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
                    
                    if (user == null)
                    {
                        return Result.Failure<UserDto>("User not found.");
                    }

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

                    // 3. Save changes to database
                    await _context.SaveChangesAsync(cancellationToken);

                    // 4. Create and save the domain event to outbox
                    var profileUpdatedEvent = new UserProfileUpdatedEvent(
                        user.Id,
                        user.DisplayName,
                        user.Gender?.ToString(),
                        user.DateOfBirth,
                        user.UnitPreference.ToString(),
                        user.UpdatedAt);

                    var outboxMessage = new OutboxMessageEntity
                    {
                        EventId = Guid.NewGuid().ToString(),
                        EventType = nameof(UserProfileUpdatedEvent),
                        EventData = JsonSerializer.Serialize(profileUpdatedEvent),
                        CreatedAt = DateTime.UtcNow,
                        IsProcessed = false
                    };

                    _context.OutboxMessages.Add(outboxMessage);
                    await _context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                    
                    _logger.LogInformation("Successfully updated profile for user {UserId}", user.Id);
                    return Result.Success(UserMapper.ToDto(user));
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user {UserId}: {ErrorMessage}", request.UserId, ex.Message);
            return Result.Failure<UserDto>($"Failed to update user profile: {ex.Message}");
        }
    }
}