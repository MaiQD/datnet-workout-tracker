using DatNetWorkoutTracker.Users.Domain;

namespace DatNetWorkoutTracker.Users.Services;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByGoogleIdAsync(string googleId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserFromGoogleAsync(GoogleUserInfo googleUserInfo);
    Task<User> UpdateUserAsync(User user);
    Task UpdateLastLoginAsync(string userId);
    Task<bool> UserExistsAsync(string userId);
}

public class GoogleUserInfo
{
    public string GoogleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
}
