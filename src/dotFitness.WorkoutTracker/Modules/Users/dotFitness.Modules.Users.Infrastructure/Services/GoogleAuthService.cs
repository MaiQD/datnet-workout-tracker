using Microsoft.Extensions.Logging;
using System.Text.Json;
using dotFitness.Modules.Users.Application.Services;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GoogleAuthService(ILogger<GoogleAuthService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get user info from Google API: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Google API response received for token: {TokenPrefix}", 
                accessToken[..Math.Min(10, accessToken.Length)]);
            
            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(json, _jsonOptions);
            
            // Google API returns "picture" field for profile image URL
            if (userInfo != null && json.Contains("\"picture\""))
            {
                // Parse the picture URL manually since our record doesn't have it yet
                var jsonDoc = JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("picture", out var pictureElement))
                {
                    var pictureUrl = pictureElement.GetString();
                    // Create new instance with profile picture
                    userInfo = new GoogleUserInfo(userInfo.Id, userInfo.Email, userInfo.Name, pictureUrl);
                }
            }
            if (userInfo != null)
            {
                _logger.LogInformation("Successfully retrieved Google user info: {Email}", userInfo.Email);
            }
            
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info from Google API");
            return null;
        }
    }
}
