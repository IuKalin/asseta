using Asseta.Application.Features.SafeActivation.DTOs;
using MediatR;

namespace Asseta.Application.Features.SafeActivation.Queries.GetActivationStatus;

public record GetActivationStatusQuery(Guid CurrentUserId, Guid? TargetOwnerId = null) : IRequest<ActivationStatusDto>;
