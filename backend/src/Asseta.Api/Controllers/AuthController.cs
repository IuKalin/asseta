using Asseta.Api.Models;
using Asseta.Application.Features.Auth.Commands.Login;
using Asseta.Application.Features.Auth.Commands.RefreshToken;
using Asseta.Application.Features.Auth.Commands.Register;
using Asseta.Application.Features.Auth.DTOs;
using Asseta.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asseta.Api.Controllers;

[Route("api/v1/auth")]
[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FullName,
            request.PhoneNumber);

        var result = await _mediator.Send(command);
        return CreatedResponse("/api/v1/auth/me", result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetMe()
    {
        var query = new GetCurrentUserQuery(CurrentOwnerId);
        var result = await _mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("verify-master-key")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<VerifyMasterKeyResponseDto>>> VerifyMasterKey([FromBody] VerifyMasterKeyRequestDto request)
    {
        var command = new Asseta.Application.Features.Auth.Commands.VerifyMasterKey.VerifyMasterKeyCommand(CurrentOwnerId, request.MasterKey);
        var result = await _mediator.Send(command);
        return OkResponse(result);
    }
}
