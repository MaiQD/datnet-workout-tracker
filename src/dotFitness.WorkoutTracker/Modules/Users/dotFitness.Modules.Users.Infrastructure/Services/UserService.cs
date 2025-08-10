using dotFitness.Modules.Users.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly AdminSettings _adminSettings;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IOptions<AdminSettings> adminSettings,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _adminSettings = adminSettings.Value;
        _logger = logger;
    }

    public async Task<Result<User>> GetOrCreateUserAsync(GoogleUserInfo googleUserInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user exists
            var existingUserResult = await _userRepository.GetByGoogleIdAsync(googleUserInfo.Id, cancellationToken);
            if (existingUserResult.IsSuccess)
            {
                var user = existingUserResult.Value!;
                
                // Update profile picture only if it has actually changed
                if (!string.Equals(user.ProfilePicture, googleUserInfo.ProfilePicture, StringComparison.Ordinal))
                {
                    user.ProfilePicture = googleUserInfo.ProfilePicture;
                    user.UpdatedAt = DateTime.UtcNow;
                    
                    var updateResult = await _userRepository.UpdateAsync(user, cancellationToken);
                    if (updateResult.IsFailure)
                    {
                        _logger.LogWarning("Failed to update user profile picture: {Error}", updateResult.Error);
                    }
                }
                
                _logger.LogInformation("Existing user logged in: {Email}", user.Email);
                return Result.Success(user);
            }

            // Create new user
            var newUser = CreateNewUser(googleUserInfo);
            
            var createResult = await _userRepository.CreateAsync(newUser, cancellationToken);
            if (createResult.IsFailure)
            {
                return Result.Failure<User>(createResult.Error!);
            }

            var createdUser = createResult.Value!;
            _logger.LogInformation("New user created: {Email}", createdUser.Email);
            
            return Result.Success(createdUser);
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
            Id = ObjectId.GenerateNewId().ToString(),
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
