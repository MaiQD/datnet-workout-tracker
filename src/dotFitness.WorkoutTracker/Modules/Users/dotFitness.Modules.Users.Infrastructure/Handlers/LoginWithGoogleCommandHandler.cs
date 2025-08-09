using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginWithGoogleCommandHandler> _logger;

    public LoginWithGoogleCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserService userService,
        IJwtService jwtService,
        ILogger<LoginWithGoogleCommandHandler> logger)
    {
        _googleAuthService = googleAuthService;
        _userService = userService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user info from Google
            var userInfo = await _googleAuthService.GetUserInfoAsync(request.Request.GoogleToken, cancellationToken);
            if (userInfo == null)
            {
                return Result.Failure<LoginResponseDto>("Failed to get user information from Google");
            }

            // Get or create user
            var userResult = await _userService.GetOrCreateUserAsync(userInfo, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<LoginResponseDto>(userResult.Error!);
            }

            var user = userResult.Value!;

            // Generate JWT token and response
            var token = _jwtService.GenerateToken(user);
            var expiresAt = _jwtService.GetExpirationTime();

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = user.Roles.ToList(),
                ExpiresAt = expiresAt
            };

            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during Google login");
            return Result.Failure<LoginResponseDto>($"Login failed: {ex.Message}");
        }
    }
}