using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IUserPreferencesProjectionRepository
{
    Task<Result<UserPreferencesProjection?>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}


