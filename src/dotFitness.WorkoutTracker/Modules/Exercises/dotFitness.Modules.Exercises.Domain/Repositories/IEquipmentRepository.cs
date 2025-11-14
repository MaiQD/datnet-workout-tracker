using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Domain.Repositories;

public interface IEquipmentRepository
{
    Task<Result<Equipment>> CreateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Result<Equipment?>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetGlobalEquipmentAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Equipment>>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Equipment?>> GetByNameAsync(string name, Guid? userId = null,
        CancellationToken cancellationToken = default);
    Task<Result<Equipment>> UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> UserOwnsEquipmentAsync(string equipmentId, Guid userId,
        CancellationToken cancellationToken = default);
}
