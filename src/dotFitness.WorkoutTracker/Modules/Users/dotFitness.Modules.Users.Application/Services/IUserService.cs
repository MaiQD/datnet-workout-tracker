using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Application.Services;

public interface IUserService
{
    Task<Result<User>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default);
}
