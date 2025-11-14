using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.Repositories;

public class UserRepository(UsersDbContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<Result<ApplicationUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        return user == null ? Result.Failure<ApplicationUser>("User not found") : Result.Success(user);
    }

    public async Task<Result<ApplicationUser>> GetByEmailAsync(string email,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user == null ? Result.Failure<ApplicationUser>("User not found") : Result.Success(user);
    }

    public async Task<Result<ApplicationUser>> GetByGoogleIdAsync(string googleId,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);

        return user != null ? Result.Success(user) : Result.Failure<ApplicationUser>("User not found");
    }

    public async Task<Result<ApplicationUser>> CreateAsync(ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
        return Result.Success(user);
    }

    public async Task<Result<ApplicationUser>> UpdateAsync(ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully updated user with ID: {UserId}", user.Id);
        return Result.Success(user);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
        return Result.Success();
    }

    public async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await context.Users.AnyAsync(u => u.Id == id, cancellationToken);
        return Result.Success(exists);
    }

    public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        return Result.Success(exists);
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> GetAllAsync(int skip = 0, int take = 50,
        CancellationToken cancellationToken = default)
    {
        var users = await context.Users
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return Result.Success(users.AsEnumerable());
    }

    public async Task<Result<long>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await context.Users.LongCountAsync(cancellationToken);
        return Result.Success(count);
    }

}