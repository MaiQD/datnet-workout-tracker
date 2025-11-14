using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (userResult.IsFailure)
        {
            return Result.Failure<UserDto>(userResult.Error ?? "User not found");
        }

        var userDto = UserMapper.ToDto(userResult.Value!);
        return Result.Success(userDto);
    }
}

