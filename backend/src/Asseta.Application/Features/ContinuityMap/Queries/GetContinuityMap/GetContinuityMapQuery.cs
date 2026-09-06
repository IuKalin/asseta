using Asseta.Application.Features.ContinuityMap.DTOs;
using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;

public record GetContinuityMapQuery(Guid OwnerId) : IRequest<ContinuityMapDto>;
