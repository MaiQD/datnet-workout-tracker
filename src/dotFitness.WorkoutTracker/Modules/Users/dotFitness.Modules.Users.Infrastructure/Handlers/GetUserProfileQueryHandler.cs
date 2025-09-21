using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly UsersDbContext _context;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        UsersDbContext context,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
            if (user == null)
            {
                return Result.Failure<UserDto>("User profile not found");
            }

            var userDto = UserMapper.ToDto(user);
            return Result.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile for user {UserId}", request.UserId);
            return Result.Failure<UserDto>($"Failed to get user profile: {ex.Message}");
        }
    }
}
