using DatNetWorkoutTracker.Shared.Domain;
using DatNetWorkoutTracker.Users.Domain;

namespace DatNetWorkoutTracker.Users.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetUserByGoogleIdAsync(string googleId)
    {
        var users = await _userRepository.FindAsync(u => u.GoogleId == googleId);
        return users.FirstOrDefault();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        return users.FirstOrDefault();
    }

    public async Task<User> CreateUserFromGoogleAsync(GoogleUserInfo googleUserInfo)
    {
        var user = new User
        {
            GoogleId = googleUserInfo.GoogleId,
            Email = googleUserInfo.Email,
            FirstName = googleUserInfo.FirstName,
            LastName = googleUserInfo.LastName,
            ProfilePictureUrl = googleUserInfo.ProfilePictureUrl,
            LastLoginAt = DateTime.UtcNow
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateLastLoginAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _userRepository.ExistsAsync(userId);
    }
}
