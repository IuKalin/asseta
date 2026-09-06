using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPersonDetail;

public record GetTrustedPersonDetailQuery(
    Guid Id,
    Guid OwnerId) : IRequest<TrustedPersonDto>;
