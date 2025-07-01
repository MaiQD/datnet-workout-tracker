using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IMuscleGroupRepository
{
    Task<Result<MuscleGroup>> CreateAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default);
    Task<Result<MuscleGroup?>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MuscleGroup>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MuscleGroup>>> GetGlobalMuscleGroupsAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MuscleGroup>>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<MuscleGroup?>> GetByNameAsync(string name, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<MuscleGroup>> UpdateAsync(MuscleGroup muscleGroup, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> UserOwnsMuscleGroupAsync(string muscleGroupId, string userId, CancellationToken cancellationToken = default);
}
