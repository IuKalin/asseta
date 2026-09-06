using Asseta.Application.Features.Auth.DTOs;
using MediatR;

namespace Asseta.Application.Features.Auth.Commands.VerifyMasterKey;

public record VerifyMasterKeyCommand(
    Guid UserId,
    string MasterKey) : IRequest<VerifyMasterKeyResponseDto>;
