using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Domain.Repositories;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<Result<User>> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<bool>> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<User>>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Result<long>> GetCountAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<User>>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
}
