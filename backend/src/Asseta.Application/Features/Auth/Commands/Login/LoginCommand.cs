using Asseta.Application.Features.Auth.DTOs;
using MediatR;

namespace Asseta.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;
