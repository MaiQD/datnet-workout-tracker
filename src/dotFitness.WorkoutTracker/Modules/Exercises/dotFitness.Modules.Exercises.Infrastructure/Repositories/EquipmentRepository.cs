using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly IMongoCollection<Equipment> _equipment;
    private readonly ILogger<EquipmentRepository> _logger;

    public EquipmentRepository(IMongoDatabase database, ILogger<EquipmentRepository> logger)
    {
        _equipment = database.GetCollection<Equipment>("equipment");
        _logger = logger;
    }

    public async Task<Result<Equipment>> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        try
        {
            await _equipment.InsertOneAsync(equipment, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully created equipment with ID: {EquipmentId}", equipment.Id);
            return Result.Success(equipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create equipment {EquipmentName}. Error: {ErrorMessage}", equipment.Name, ex.Message);
            return Result.Failure<Equipment>($"Failed to create equipment: {ex.Message}");
        }
    }

    public async Task<Result<Equipment?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var equipment = await _equipment.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<Equipment?>(equipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipment by ID {EquipmentId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<Equipment?>($"Failed to get equipment: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Equipment>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var equipment = await _equipment
                .Find(e => e.UserId == userId && !e.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(equipment.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipment for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<Equipment>>($"Failed to get user equipment: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Equipment>>> GetGlobalEquipmentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var equipment = await _equipment
                .Find(e => e.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(equipment.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get global equipment. Error: {ErrorMessage}", ex.Message);
            return Result.Failure<IEnumerable<Equipment>>($"Failed to get global equipment: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Equipment>>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var equipment = await _equipment
                .Find(e => e.IsGlobal || e.UserId == userId)
                .ToListAsync(cancellationToken);
            
            return Result.Success(equipment.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all equipment for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<Equipment>>($"Failed to get equipment for user: {ex.Message}");
        }
    }

    public async Task<Result<Equipment>> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        try
        {
            equipment.UpdatedAt = DateTime.UtcNow;
            var result = await _equipment.ReplaceOneAsync(e => e.Id == equipment.Id, equipment, cancellationToken: cancellationToken);
            
            return result.ModifiedCount > 0 
                ? Result.Success(equipment) 
                : Result.Failure<Equipment>("Equipment not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update equipment with ID {EquipmentId}. Error: {ErrorMessage}", equipment.Id, ex.Message);
            return Result.Failure<Equipment>($"Failed to update equipment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _equipment.DeleteOneAsync(e => e.Id == id, cancellationToken);
            return result.DeletedCount > 0 
                ? Result.Success() 
                : Result.Failure("Equipment not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete equipment with ID {EquipmentId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure($"Failed to delete equipment: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _equipment.CountDocumentsAsync(e => e.Id == id, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check equipment existence for ID {EquipmentId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<bool>($"Failed to check equipment existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> NameExistsAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Equipment>.Filter.Eq(e => e.Name, name);
            if (!string.IsNullOrEmpty(excludeId))
            {
                filter = Builders<Equipment>.Filter.And(filter, 
                    Builders<Equipment>.Filter.Ne(e => e.Id, excludeId));
            }

            var count = await _equipment.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check equipment name existence for {Name}. Error: {ErrorMessage}", name, ex.Message);
            return Result.Failure<bool>($"Failed to check equipment name existence: {ex.Message}");
        }
    }

    public async Task<Result<Equipment?>> GetByNameAsync(string name, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            FilterDefinition<Equipment> filter;
            if (string.IsNullOrEmpty(userId))
            {
                // Search only global equipment
                filter = Builders<Equipment>.Filter.And(
                    Builders<Equipment>.Filter.Eq(e => e.Name, name),
                    Builders<Equipment>.Filter.Eq(e => e.IsGlobal, true)
                );
            }
            else
            {
                // Search global equipment or user-specific ones
                filter = Builders<Equipment>.Filter.And(
                    Builders<Equipment>.Filter.Eq(e => e.Name, name),
                    Builders<Equipment>.Filter.Or(
                        Builders<Equipment>.Filter.Eq(e => e.IsGlobal, true),
                        Builders<Equipment>.Filter.Eq(e => e.UserId, userId)
                    )
                );
            }

            var equipment = await _equipment.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<Equipment?>(equipment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipment by name {Name} for user {UserId}. Error: {ErrorMessage}", name, userId, ex.Message);
            return Result.Failure<Equipment?>($"Failed to get equipment by name: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserOwnsEquipmentAsync(string equipmentId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _equipment.CountDocumentsAsync(
                e => e.Id == equipmentId && (e.IsGlobal || e.UserId == userId), 
                cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check equipment ownership for ID {EquipmentId} and user {UserId}. Error: {ErrorMessage}", equipmentId, userId, ex.Message);
            return Result.Failure<bool>($"Failed to check equipment ownership: {ex.Message}");
        }
    }
}
