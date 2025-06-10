using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IMongoDatabase database, ILogger<UserRepository> logger)
    {
        _users = database.GetCollection<User>("users");
        _logger = logger;
    }

    public async Task<Result<User>> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}. Error: {ErrorMessage}", user.Email, ex.Message);
            return Result.Failure<User>($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
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
            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
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
            var user = await _users.Find(u => u.GoogleId == googleId).FirstOrDefaultAsync(cancellationToken);
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
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);
            
            return result.ModifiedCount > 0 
                ? Result.Success(user) 
                : Result.Failure<User>("User not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user with ID {UserId}. Error: {ErrorMessage}", user.Id, ex.Message);
            return Result.Failure<User>($"Failed to update user: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id, cancellationToken);
            return result.DeletedCount > 0 
                ? Result.Success() 
                : Result.Failure("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user with ID {UserId}. Error: {ErrorMessage}", id, ex.Message);
            return Result.Failure($"Failed to delete user: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _users.CountDocumentsAsync(u => u.Id == id, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
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
            var count = await _users.CountDocumentsAsync(u => u.Email == email, cancellationToken: cancellationToken);
            return Result.Success(count > 0);
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
            var users = await _users
                .Find(_ => true)
                .Skip(skip)
                .Limit(take)
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
            var count = await _users.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
            return Result.Success(count);
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
            var users = await _users.Find(u => u.Roles.Contains(role)).ToListAsync(cancellationToken);
            return Result.Success(users.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users by role {Role}. Error: {ErrorMessage}", role, ex.Message);
            return Result.Failure<IEnumerable<User>>($"Failed to get users by role: {ex.Message}");
        }
    }
}
