using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCardById;

public record GetActionCardByIdQuery(Guid CardId, Guid OwnerId) : IRequest<ActionCardDto>;
