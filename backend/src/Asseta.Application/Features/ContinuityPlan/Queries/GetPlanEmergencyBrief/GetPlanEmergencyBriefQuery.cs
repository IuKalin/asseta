using Asseta.Application.Features.ContinuityPlan.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetPlanEmergencyBrief;

public record GetPlanEmergencyBriefQuery(Guid OwnerId) : IRequest<OfflineEmergencyBriefDto>;
