using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Commands.ClaimPairingCode;

public record ClaimPairingCodeCommand(
    Guid DelegateUserId,
    string PairingCode) : IRequest<ClaimPairingResultDto>;
