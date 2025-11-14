using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Domain.Repositories;

public interface IUserMetricsRepository
{
    Task<Result<UserMetric>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> CreateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> UpdateAsync(UserMetric userMetric, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserMetric>>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Result<long>> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsForUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    Task<Result<UserMetric>> GetByUserIdAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
}
