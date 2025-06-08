using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using DatNetWorkoutTracker.Shared.Configuration;
using DatNetWorkoutTracker.Users.Domain;

namespace DatNetWorkoutTracker.Users.Services;

public interface IGoogleAuthService
{
    string GetGoogleAuthUrl(string state);
    Task<GoogleUserInfo?> GetUserInfoFromTokenAsync(string accessToken);
    Task<GoogleUserInfo?> ProcessGoogleCallbackAsync(ClaimsPrincipal principal);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleOAuthSettings _googleSettings;
    private readonly HttpClient _httpClient;

    public GoogleAuthService(IOptions<GoogleOAuthSettings> googleSettings, HttpClient httpClient)
    {
        _googleSettings = googleSettings.Value;
        _httpClient = httpClient;
    }

    public string GetGoogleAuthUrl(string state)
    {
        var scopes = "openid email profile";
        var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                     $"client_id={_googleSettings.ClientId}&" +
                     $"redirect_uri={Uri.EscapeDataString(_googleSettings.RedirectUri)}&" +
                     $"response_type=code&" +
                     $"scope={Uri.EscapeDataString(scopes)}&" +
                     $"state={state}";

        return authUrl;
    }

    public async Task<GoogleUserInfo?> GetUserInfoFromTokenAsync(string accessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<GoogleUserApiResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (userInfo != null)
                {
                    return new GoogleUserInfo
                    {
                        GoogleId = userInfo.Id,
                        Email = userInfo.Email,
                        FirstName = userInfo.GivenName ?? "",
                        LastName = userInfo.FamilyName ?? "",
                        ProfilePictureUrl = userInfo.Picture
                    };
                }
            }
        }
        catch (Exception)
        {
            // Log error
        }

        return null;
    }

    public async Task<GoogleUserInfo?> ProcessGoogleCallbackAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated == true)
        {
            var googleId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;
            var picture = principal.FindFirst("picture")?.Value;

            if (!string.IsNullOrEmpty(googleId) && !string.IsNullOrEmpty(email))
            {
                return new GoogleUserInfo
                {
                    GoogleId = googleId,
                    Email = email,
                    FirstName = firstName ?? "",
                    LastName = lastName ?? "",
                    ProfilePictureUrl = picture
                };
            }
        }

        return null;
    }

    private class GoogleUserApiResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Picture { get; set; }
    }
}
