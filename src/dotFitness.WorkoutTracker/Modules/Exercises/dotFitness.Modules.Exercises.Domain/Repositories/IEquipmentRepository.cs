using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Result<Equipment>> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Result<Equipment?>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetGlobalEquipmentAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetAllForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<Equipment?>> GetByNameAsync(string name, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<Equipment>> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> UserOwnsEquipmentAsync(string equipmentId, string userId, CancellationToken cancellationToken = default);
}
