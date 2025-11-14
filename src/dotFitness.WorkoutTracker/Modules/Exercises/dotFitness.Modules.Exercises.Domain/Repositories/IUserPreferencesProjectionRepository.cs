using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IUserPreferencesProjectionRepository
{
    Task<Result<UserPreferencesProjection?>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}


