using dotFitness.Modules.Users.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UsersDbContext _context;
    private readonly AdminSettings _adminSettings;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UsersDbContext context,
        IOptions<AdminSettings> adminSettings,
        ILogger<UserService> logger)
    {
        _context = context;
        _adminSettings = adminSettings.Value;
        _logger = logger;
    }

    public async Task<Result<User>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use execution strategy to handle retries and transactions together
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                
                try
                {
                    // Check if user exists by GoogleId
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.GoogleId == googleUserInfo.Id, cancellationToken);
                    
                    if (existingUser != null)
                    {
                        // Update profile picture only if it has actually changed
                        if (!string.Equals(existingUser.ProfilePicture, googleUserInfo.ProfilePicture, StringComparison.Ordinal))
                        {
                            existingUser.ProfilePicture = googleUserInfo.ProfilePicture;
                            existingUser.UpdatedAt = DateTime.UtcNow;
                            
                            await _context.SaveChangesAsync(cancellationToken);
                            _logger.LogInformation("Updated profile picture for user: {Email}", existingUser.Email);
                        }
                        
                        await transaction.CommitAsync(cancellationToken);
                        _logger.LogInformation("Existing user logged in: {Email}", existingUser.Email);
                        return Result.Success(existingUser);
                    }

                    // Create new user
                    var newUser = CreateNewUser(googleUserInfo);
                    
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync(cancellationToken);
                    
                    await transaction.CommitAsync(cancellationToken);
                    _logger.LogInformation("New user created: {Email}", newUser.Email);
                    
                    return Result.Success(newUser);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get or create user for email: {Email}", googleUserInfo.Email);
            return Result.Failure<User>($"User management failed: {ex.Message}");
        }
    }

    private User CreateNewUser(GoogleUserInfo googleUserInfo)
    {
        var user = new User
        {
            GoogleId = googleUserInfo.Id,
            Email = googleUserInfo.Email,
            DisplayName = googleUserInfo.Name,
            ProfilePicture = googleUserInfo.ProfilePicture,
            LoginMethod = LoginMethod.Google,
            Roles = new List<string> { "User" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Check if user should be admin
        if (_adminSettings.AdminEmails.Contains(googleUserInfo.Email))
        {
            user.Roles.Add("Admin");
            _logger.LogInformation("Admin user created: {Email}", user.Email);
        }

        return user;
    }
}
