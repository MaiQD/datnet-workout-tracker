using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(UsersDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<User>> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}. Error: {ErrorMessage}", user.Email, ex.Message);
            return Result.Failure<User>($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            return user != null 
                ? Result.Success(user) 
                : Result.Failure<User>("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by ID {UserId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<User>($"Failed to get user: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            return user != null 
                ? Result.Success(user) 
                : Result.Failure<User>("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by email {Email}. Error: {ErrorMessage}", email, ex.Message);
            return Result.Failure<User>($"Failed to get user by email: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);
            return user != null 
                ? Result.Success(user) 
                : Result.Failure<User>("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by Google ID {GoogleId}. Error: {ErrorMessage}", googleId, ex.Message);
            return Result.Failure<User>($"Failed to get user by Google ID: {ex.Message}");
        }
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Users.Update(user);
            var affectedRows = await _context.SaveChangesAsync(cancellationToken);
            
            return affectedRows > 0 
                ? Result.Success(user) 
                : Result.Failure<User>("User not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user with ID {UserId}. Error: {ErrorMessage}", user.Id, ex.Message);
            return Result.Failure<User>($"Failed to update user: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
                return Result.Failure("User not found");

            _context.Users.Remove(user);
            var affectedRows = await _context.SaveChangesAsync(cancellationToken);
            return affectedRows > 0 
                ? Result.Success() 
                : Result.Failure("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user with ID {UserId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure($"Failed to delete user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check user existence for ID {UserId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure<bool>($"Failed to check user existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check email existence for {Email}. Error: {ErrorMessage}", email, ex.Message);
            return Result.Failure<bool>($"Failed to check email existence: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _context.Users
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
            
            return Result.Success(users.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all users with skip {Skip} and take {Take}. Error: {ErrorMessage}", skip, take, ex.Message);
            return Result.Failure<IEnumerable<User>>($"Failed to get all users: {ex.Message}");
        }
    }

    public async Task<Result<long>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _context.Users.CountAsync(cancellationToken);
            return Result.Success((long)count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user count. Error: {ErrorMessage}", ex.Message);
            return Result.Failure<long>($"Failed to get user count: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<User>>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _context.Users
                .Where(u => u.Roles.Contains(role))
                .ToListAsync(cancellationToken);
            return Result.Success(users.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users by role {Role}. Error: {ErrorMessage}", role, ex.Message);
            return Result.Failure<IEnumerable<User>>($"Failed to get users by role: {ex.Message}");
        }
    }
}
