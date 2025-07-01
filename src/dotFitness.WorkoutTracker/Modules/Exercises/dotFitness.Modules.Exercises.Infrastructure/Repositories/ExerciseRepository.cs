using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly IMongoCollection<Exercise> _exercises;
    private readonly ILogger<ExerciseRepository> _logger;

    public ExerciseRepository(IMongoDatabase database, ILogger<ExerciseRepository> logger)
    {
        _exercises = database.GetCollection<Exercise>("exercises");
        _logger = logger;
    }

    public async Task<Result<Exercise>> CreateAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            await _exercises.InsertOneAsync(exercise, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully created exercise with ID: {ExerciseId}", exercise.Id);
            return Result.Success(exercise);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create exercise {ExerciseName}. Error: {ErrorMessage}", exercise.Name, ex.Message);
            return Result.Failure<Exercise>($"Failed to create exercise: {ex.Message}");
        }
    }

    public async Task<Result<Exercise?>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exercise = await _exercises.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<Exercise?>(exercise);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercise by ID {ExerciseId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<Exercise?>($"Failed to get exercise: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Exercise>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var exercises = await _exercises
                .Find(e => e.UserId == userId && !e.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(exercises.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exercises for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<Exercise>>($"Failed to get user exercises: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Exercise>>> GetGlobalExercisesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var exercises = await _exercises
                .Find(e => e.IsGlobal)
                .ToListAsync(cancellationToken);
            
            return Result.Success(exercises.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get global exercises. Error: {ErrorMessage}", ex.Message);
            return Result.Failure<IEnumerable<Exercise>>($"Failed to get global exercises: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Exercise>>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var exercises = await _exercises
                .Find(e => e.IsGlobal || e.UserId == userId)
                .ToListAsync(cancellationToken);
            
            return Result.Success(exercises.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all exercises for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<Exercise>>($"Failed to get exercises for user: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Exercise>>> SearchAsync(string userId, string? searchTerm = null,
        List<string>? muscleGroups = null, List<string>? equipment = null,
        ExerciseDifficulty? difficulty = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterBuilder = Builders<Exercise>.Filter;
            var filters = new List<FilterDefinition<Exercise>>
            {
                filterBuilder.Or(
                    filterBuilder.Eq(e => e.IsGlobal, true),
                    filterBuilder.Eq(e => e.UserId, userId)
                )
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filters.Add(filterBuilder.Or(
                    filterBuilder.Regex(e => e.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    filterBuilder.Regex(e => e.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                ));
            }

            if (muscleGroups?.Any() == true)
            {
                filters.Add(filterBuilder.AnyIn(e => e.MuscleGroups, muscleGroups));
            }

            if (equipment?.Any() == true)
            {
                filters.Add(filterBuilder.AnyIn(e => e.Equipment, equipment));
            }

            if (difficulty.HasValue)
            {
                filters.Add(filterBuilder.Eq(e => e.Difficulty, difficulty.Value));
            }

            var combinedFilter = filterBuilder.And(filters);
            var exercises = await _exercises
                .Find(combinedFilter)
                .ToListAsync(cancellationToken);

            return Result.Success(exercises.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search exercises for user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
            return Result.Failure<IEnumerable<Exercise>>($"Failed to search exercises: {ex.Message}");
        }
    }

    public async Task<Result<Exercise>> UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            exercise.UpdatedAt = DateTime.UtcNow;
            var result = await _exercises.ReplaceOneAsync(e => e.Id == exercise.Id, exercise, cancellationToken: cancellationToken);
            
            return result.ModifiedCount > 0 
                ? Result.Success(exercise) 
                : Result.Failure<Exercise>("Exercise not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update exercise with ID {ExerciseId}. Error: {ErrorMessage}", exercise.Id, ex.Message);
            return Result.Failure<Exercise>($"Failed to update exercise: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _exercises.DeleteOneAsync(e => e.Id == id, cancellationToken);
            return result.DeletedCount > 0 
                ? Result.Success() 
                : Result.Failure("Exercise not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete exercise with ID {ExerciseId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure($"Failed to delete exercise: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _exercises.CountDocumentsAsync(e => e.Id == id, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check exercise existence for ID {ExerciseId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<bool>($"Failed to check exercise existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserOwnsExerciseAsync(string exerciseId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _exercises.CountDocumentsAsync(
                e => e.Id == exerciseId && (e.IsGlobal || e.UserId == userId), 
                cancellationToken: cancellationToken);
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check exercise ownership for ID {ExerciseId} and user {UserId}. Error: {ErrorMessage}", exerciseId, userId, ex.Message);
            return Result.Failure<bool>($"Failed to check exercise ownership: {ex.Message}");
        }
    }
}
