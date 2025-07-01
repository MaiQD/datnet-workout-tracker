using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Repositories;

public class MuscleGroupRepository : IMuscleGroupRepository
{
    private readonly IMongoCollection<MuscleGroup> _muscleGroups;
    private readonly ILogger<MuscleGroupRepository> _logger;

    public MuscleGroupRepository(IMongoDatabase database, ILogger<MuscleGroupRepository> logger)
    {
        _muscleGroups = database.GetCollection<MuscleGroup>("muscleGroups");
        _logger = logger;
    }

    public async Task<Result<MuscleGroup>> CreateAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default)
    {
        try
        {
            await _muscleGroups.InsertOneAsync(muscleGroup, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully created muscle group with ID: {MuscleGroupId}", muscleGroup.Id);
            return Result.Success(muscleGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create muscle group {MuscleGroupName}. Error: {ErrorMessage}", muscleGroup.Name, ex.Message);
            return Result.Failure<MuscleGroup>($"Failed to create muscle group: {ex.Message}");
        }
    }

    public async Task<Result<MuscleGroup?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var muscleGroup = await _muscleGroups.Find(mg => mg.Id == id).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<MuscleGroup?>(muscleGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get muscle group by ID {MuscleGroupId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<MuscleGroup?>($"Failed to get muscle group: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MuscleGroup>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var muscleGroups = await _muscleGroups
                .Find(mg => mg.UserId == userId && !mg.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(muscleGroups.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get muscle groups for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<MuscleGroup>>($"Failed to get user muscle groups: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MuscleGroup>>> GetGlobalMuscleGroupsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var muscleGroups = await _muscleGroups
                .Find(mg => mg.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(muscleGroups.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get global muscle groups. Error: {ErrorMessage}", ex.Message);
            return Result.Failure<IEnumerable<MuscleGroup>>($"Failed to get global muscle groups: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MuscleGroup>>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var muscleGroups = await _muscleGroups
                .Find(mg => mg.IsGlobal || mg.UserId == userId)
                .ToListAsync(cancellationToken);
            
            return Result.Success(muscleGroups.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all muscle groups for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<MuscleGroup>>($"Failed to get muscle groups for user: {ex.Message}");
        }
    }

    public async Task<Result<MuscleGroup>> UpdateAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default)
    {
        try
        {
            muscleGroup.UpdatedAt = DateTime.UtcNow;
            var result = await _muscleGroups.ReplaceOneAsync(mg => mg.Id == muscleGroup.Id, muscleGroup, cancellationToken: cancellationToken);
            
            return result.ModifiedCount > 0 
                ? Result.Success(muscleGroup) 
                : Result.Failure<MuscleGroup>("Muscle group not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update muscle group with ID {MuscleGroupId}. Error: {ErrorMessage}", muscleGroup.Id, ex.Message);
            return Result.Failure<MuscleGroup>($"Failed to update muscle group: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _muscleGroups.DeleteOneAsync(mg => mg.Id == id, cancellationToken);
            return result.DeletedCount > 0 
                ? Result.Success() 
                : Result.Failure("Muscle group not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete muscle group with ID {MuscleGroupId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure($"Failed to delete muscle group: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _muscleGroups.CountDocumentsAsync(mg => mg.Id == id, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check muscle group existence for ID {MuscleGroupId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<bool>($"Failed to check muscle group existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> NameExistsAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<MuscleGroup>.Filter.Eq(mg => mg.Name, name);
            if (!string.IsNullOrEmpty(excludeId))
            {
                filter = Builders<MuscleGroup>.Filter.And(filter, 
                    Builders<MuscleGroup>.Filter.Ne(mg => mg.Id, excludeId));
            }

            var count = await _muscleGroups.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check muscle group name existence for {Name}. Error: {ErrorMessage}", name, ex.Message);
            return Result.Failure<bool>($"Failed to check muscle group name existence: {ex.Message}");
        }
    }

    public async Task<Result<MuscleGroup?>> GetByNameAsync(string name, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            FilterDefinition<MuscleGroup> filter;
            if (string.IsNullOrEmpty(userId))
            {
                // Search only global muscle groups
                filter = Builders<MuscleGroup>.Filter.And(
                    Builders<MuscleGroup>.Filter.Eq(mg => mg.Name, name),
                    Builders<MuscleGroup>.Filter.Eq(mg => mg.IsGlobal, true)
                );
            }
            else
            {
                // Search global muscle groups or user-specific ones
                filter = Builders<MuscleGroup>.Filter.And(
                    Builders<MuscleGroup>.Filter.Eq(mg => mg.Name, name),
                    Builders<MuscleGroup>.Filter.Or(
                        Builders<MuscleGroup>.Filter.Eq(mg => mg.IsGlobal, true),
                        Builders<MuscleGroup>.Filter.Eq(mg => mg.UserId, userId)
                    )
                );
            }

            var muscleGroup = await _muscleGroups.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<MuscleGroup?>(muscleGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get muscle group by name {Name} for user {UserId}. Error: {ErrorMessage}", name, userId, ex.Message);
            return Result.Failure<MuscleGroup?>($"Failed to get muscle group by name: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserOwnsMuscleGroupAsync(string muscleGroupId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _muscleGroups.CountDocumentsAsync(
                mg => mg.Id == muscleGroupId && (mg.IsGlobal || mg.UserId == userId), 
                cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check muscle group ownership for ID {MuscleGroupId} and user {UserId}. Error: {ErrorMessage}", muscleGroupId, userId, ex.Message);
            return Result.Failure<bool>($"Failed to check muscle group ownership: {ex.Message}");
        }
    }
}
