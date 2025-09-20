using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;
using MongoDB.Driver;

namespace dotFitness.Modules.Exercises.Infrastructure.Repositories;

public class UserPreferencesProjectionRepository : IUserPreferencesProjectionRepository
{
    private readonly IMongoCollection<UserPreferencesProjection> _collection;

    public UserPreferencesProjectionRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserPreferencesProjection>("userPreferencesProjections");
    }

    public async Task<Result<UserPreferencesProjection?>> GetByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pref = await _collection.Find(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
            return Result.Success<UserPreferencesProjection?>(pref);
        }
        catch (Exception ex)
        {
            return Result.Failure<UserPreferencesProjection?>($"Failed to get preferences: {ex.Message}");
        }
    }
}


