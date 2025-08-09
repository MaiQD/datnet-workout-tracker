using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public interface IUserService
{
    Task<Result<User>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default);
}
