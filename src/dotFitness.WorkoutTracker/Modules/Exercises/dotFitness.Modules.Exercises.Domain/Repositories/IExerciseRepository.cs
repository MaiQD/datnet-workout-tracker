using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IExerciseRepository
{
    Task<Result<Exercise>> CreateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task<Result<Exercise?>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Exercise>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Exercise>>> GetGlobalExercisesAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Exercise>>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Exercise>>> SearchAsync(int userId, string? searchTerm = null,
        List<string>? muscleGroups = null, List<string>? equipment = null,
        ExerciseDifficulty? difficulty = null, CancellationToken cancellationToken = default);
    Task<Result<Exercise>> UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> UserOwnsExerciseAsync(string exerciseId, int userId,
        CancellationToken cancellationToken = default);
}
