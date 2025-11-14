using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Application.Services;

public interface IIdentityService
{
    Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<Result> AddToRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<string>>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<string> GenerateUserTokenAsync(ApplicationUser user, string tokenType, CancellationToken cancellationToken = default);
    Task SignInAsync(ApplicationUser user, bool isPersistent, CancellationToken cancellationToken = default);
}

