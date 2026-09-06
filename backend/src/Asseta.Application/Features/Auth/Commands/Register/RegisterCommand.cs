using Asseta.Application.Features.Auth.DTOs;
using MediatR;

namespace Asseta.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string? PhoneNumber = null) : IRequest<AuthResponseDto>;
