using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Domain.Repositories;

public interface IUserMetricsRepository
{
    Task<Result<UserMetric>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> CreateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> UpdateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAsync(string userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Result<long>> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsForUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default);
}
