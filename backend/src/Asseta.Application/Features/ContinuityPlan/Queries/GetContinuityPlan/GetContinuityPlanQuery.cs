using Asseta.Application.Features.ContinuityPlan.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetContinuityPlan;

public record GetContinuityPlanQuery(Guid OwnerId) : IRequest<ContinuityPlanDto>;
