using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Domain.Repositories;

public interface IUserRepository
{
    Task<Result<ApplicationUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ApplicationUser>>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Result<long>> GetCountAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ApplicationUser>>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
}
