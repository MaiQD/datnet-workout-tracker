namespace dotFitness.Modules.Users.Application.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
}

public record GoogleUserInfo(string Id, string Email, string Name, string? ProfilePicture);
