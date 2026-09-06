using Asseta.Application.Features.ContinuityPlan.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityPlan.Queries.GetMyDelegatedPlan;

public record GetMyDelegatedPlanQuery(Guid DelegateUserId) : IRequest<ContinuityPlanDto>;
