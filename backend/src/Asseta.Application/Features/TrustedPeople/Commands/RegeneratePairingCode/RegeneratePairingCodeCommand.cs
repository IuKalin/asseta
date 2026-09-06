using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.RegeneratePairingCode;

public record RegeneratePairingCodeCommand(
    Guid Id,
    Guid OwnerId) : IRequest<TrustedPersonDto>;
