using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class UserService(
    UsersDbContext context,
    IOptions<AdminSettings> adminSettings,
    ILogger<UserService> logger)
    : IUserService
{
    private readonly AdminSettings _adminSettings = adminSettings.Value;

    public async Task<Result<ApplicationUser>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use execution strategy to handle retries and transactions together
            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                
                try
                {
                    // Check if user exists by GoogleId
                    var existingUser = await context.Users
                        .FirstOrDefaultAsync(u => u.GoogleId == googleUserInfo.Id, cancellationToken);
                    
                    if (existingUser != null)
                    {
                        // Update profile picture only if it has actually changed
                        if (!string.Equals(existingUser.ProfilePicture, googleUserInfo.ProfilePicture, StringComparison.Ordinal))
                        {
                            existingUser.ProfilePicture = googleUserInfo.ProfilePicture;
                            existingUser.UpdatedAt = DateTime.UtcNow;
                            
                            await context.SaveChangesAsync(cancellationToken);
                            logger.LogInformation("Updated profile picture for user: {Email}", existingUser.Email);
                        }
                        
                        await transaction.CommitAsync(cancellationToken);
                        logger.LogInformation("Existing user logged in: {Email}", existingUser.Email);
                        return Result.Success(existingUser);
                    }

                    // Create new user
                    var newUser = CreateNewUser(googleUserInfo);
                    
                    context.Users.Add(newUser);
                    await context.SaveChangesAsync(cancellationToken);
                    
                    await transaction.CommitAsync(cancellationToken);
                    logger.LogInformation("New user created: {Email}", newUser.Email);
                    
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
            logger.LogError(ex, "Failed to get or create user for email: {Email}", googleUserInfo.Email);
            return Result.Failure<ApplicationUser>($"User management failed: {ex.Message}");
        }
    }

    private ApplicationUser CreateNewUser(GoogleUserInfo googleUserInfo)
    {
        var user = new ApplicationUser
        {
            GoogleId = googleUserInfo.Id,
            Email = googleUserInfo.Email,
            UserName = googleUserInfo.Email,
            DisplayName = googleUserInfo.Name,
            ProfilePicture = googleUserInfo.ProfilePicture,
            LoginMethod = LoginMethod.Google,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Check if user should be admin
        if (_adminSettings.AdminEmails.Contains(googleUserInfo.Email))
        {
            // Note: Role assignment will be handled by UserManager in the handler
            logger.LogInformation("Admin user detected: {Email}", user.Email);
        }

        return user;
    }
}
