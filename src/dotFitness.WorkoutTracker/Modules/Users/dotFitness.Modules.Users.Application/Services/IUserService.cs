using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Application.Services;

public interface IUserService
{
    Task<Result<ApplicationUser>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default);
}
