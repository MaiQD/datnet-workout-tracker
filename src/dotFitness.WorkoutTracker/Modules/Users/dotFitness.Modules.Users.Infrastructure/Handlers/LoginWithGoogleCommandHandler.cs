using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginWithGoogleCommandHandler> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly AdminSettings _adminSettings;

    public LoginWithGoogleCommandHandler(
        IUserRepository userRepository,
        ILogger<LoginWithGoogleCommandHandler> logger,
        IOptions<JwtSettings> jwtSettings,
        IOptions<AdminSettings> adminSettings)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
        _adminSettings = adminSettings.Value;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // For Google OAuth2 access tokens, we need to use the Google People API
            // to get user information instead of validating the token directly
            var userInfo = await GetGoogleUserInfoAsync(request.Request.GoogleToken);
            if (userInfo == null)
            {
                return Result.Failure<LoginResponseDto>("Failed to get user information from Google");
            }

            // Check if user exists
            var existingUserResult = await _userRepository.GetByGoogleIdAsync(userInfo.Id, cancellationToken);
            User user;

            if (existingUserResult.IsSuccess)
            {
                user = existingUserResult.Value!;
                _logger.LogInformation("Existing user logged in: {Email}", user.Email);
            }
            else
            {
                // Create new user
                user = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    GoogleId = userInfo.Id,
                    Email = userInfo.Email,
                    DisplayName = userInfo.Name,
                    LoginMethod = LoginMethod.Google,
                    Roles = new List<string> { "User" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Check if user should be admin
                if (_adminSettings.AdminEmails.Contains(userInfo.Email))
                {
                    user.Roles.Add("Admin");
                    _logger.LogInformation("Admin user created: {Email}", user.Email);
                }

                var createResult = await _userRepository.CreateAsync(user, cancellationToken);
                if (createResult.IsFailure)
                {
                    return Result.Failure<LoginResponseDto>(createResult.Error!);
                }

                user = createResult.Value!;
                _logger.LogInformation("New user created: {Email}", user.Email);
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours);

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = user.Roles.ToList(),
                ExpiresAt = expiresAt
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process Google login for token: {TokenPrefix}", 
                request.Request.GoogleToken[..Math.Min(10, request.Request.GoogleToken.Length)]);
            return Result.Failure<LoginResponseDto>($"Login failed: {ex.Message}");
        }
    }

    private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get user info from Google API: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Google API response: {Json}", json);
            
            // Use case-insensitive deserialization
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var userInfo = System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(json, options);
            _logger.LogInformation("Deserialized user info: Id={Id}, Email={Email}, Name={Name}", 
                userInfo?.Id, userInfo?.Email, userInfo?.Name);
            
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info from Google API");
            return null;
        }
    }

    private class GoogleUserInfo
    {
        public string Id { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
    }

    private string GenerateJwtToken(User user)
    {
        // Debug logging for JWT settings
        _logger.LogInformation("Generating JWT token for user {UserId}", user.Id);
        _logger.LogInformation("JWT Secret Key length: {SecretKeyLength}", _jwtSettings.SecretKey?.Length ?? 0);
        _logger.LogInformation("JWT Issuer: {Issuer}", _jwtSettings.Issuer);
        _logger.LogInformation("JWT Audience: {Audience}", _jwtSettings.Audience);
        _logger.LogInformation("JWT Expiration Hours: {ExpirationHours}", _jwtSettings.ExpirationInHours);

        if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("JWT Secret Key is null or empty");
        }

        if (string.IsNullOrEmpty(_jwtSettings.Issuer))
        {
            throw new InvalidOperationException("JWT Issuer is null or empty");
        }

        if (string.IsNullOrEmpty(_jwtSettings.Audience))
        {
            throw new InvalidOperationException("JWT Audience is null or empty");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new("GoogleId", user.GoogleId ?? string.Empty)
        };

        // Add role claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}