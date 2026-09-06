using Asseta.Application.Features.ContinuityPlan.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetPlanReadinessAudit;

public record GetPlanReadinessAuditQuery(Guid OwnerId) : IRequest<PlanReadinessAuditDto>;
