using Asseta.Application.Features.Auth.DTOs;
using MediatR;

namespace Asseta.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<AuthResponseDto>;
