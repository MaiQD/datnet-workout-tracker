using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<IdentityService> logger)
    : IIdentityService
{
    private readonly ILogger<IdentityService> _logger = logger;

    public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<Result<ApplicationUser>> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var result = await userManager.CreateAsync(user);
        if (result.Succeeded) return Result.Success(user);
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result.Failure<ApplicationUser>($"Failed to create user: {errors}");
    }

    public async Task<Result> AddToRoleAsync(ApplicationUser user, string role, CancellationToken cancellationToken = default)
    {
        var result = await userManager.AddToRoleAsync(user, role);
        if (result.Succeeded) return Result.Success();
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result.Failure($"Failed to add role: {errors}");
    }

    public async Task<Result<IEnumerable<string>>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(user);
        return Result.Success(roles.AsEnumerable());
    }

    public async Task<string> GenerateUserTokenAsync(ApplicationUser user, string tokenType, CancellationToken cancellationToken = default)
    {
        return await userManager.GenerateUserTokenAsync(user, "Default", tokenType);
    }

    public async Task SignInAsync(ApplicationUser user, bool isPersistent, CancellationToken cancellationToken = default)
    {
        await signInManager.SignInAsync(user, isPersistent: isPersistent);
    }
}

