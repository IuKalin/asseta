using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Enums;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCards;

public record GetActionCardsQuery(
    Guid OwnerId,
    UrgencyStage? Urgency = null,
    Guid? CategoryId = null,
    string? SearchQuery = null) : IRequest<IReadOnlyList<ActionCardDto>>;
