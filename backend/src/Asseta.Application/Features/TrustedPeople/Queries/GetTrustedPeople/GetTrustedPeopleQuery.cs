using Asseta.Application.Features.TrustedPeople.DTOs;
using MediatR;

namespace Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPeople;

public record GetTrustedPeopleQuery(Guid OwnerId) : IRequest<List<TrustedPersonDto>>;
