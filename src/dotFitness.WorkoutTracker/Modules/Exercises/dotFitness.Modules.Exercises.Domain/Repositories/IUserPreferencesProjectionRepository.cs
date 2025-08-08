using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IUserPreferencesProjectionRepository
{
    Task<Result<UserPreferencesProjection?>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}


