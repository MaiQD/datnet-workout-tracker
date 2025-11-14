using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (userResult.IsFailure)
        {
            return Result.Failure<UserDto>(userResult.Error ?? "User profile not found");
        }

        var userDto = UserMapper.ToDto(userResult.Value!);
        return Result.Success(userDto);
    }
}

