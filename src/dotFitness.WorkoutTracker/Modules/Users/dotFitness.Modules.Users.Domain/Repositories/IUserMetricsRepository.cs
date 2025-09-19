using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Domain.Repositories;

public interface IUserMetricsRepository
{
    Task<Result<UserMetric>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> CreateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> UpdateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAsync(int userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetLatestByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Result<long>> GetCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsForUserAndDateAsync(int userId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetByUserIdAndDateAsync(int userId, DateTime date, CancellationToken cancellationToken = default);
}
