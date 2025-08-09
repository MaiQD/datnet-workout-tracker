namespace dotFitness.Modules.Users.Infrastructure.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
}

public record GoogleUserInfo(string Id, string Email, string Name);
