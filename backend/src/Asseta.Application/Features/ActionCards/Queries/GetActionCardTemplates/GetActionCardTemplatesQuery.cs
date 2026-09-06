using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCardTemplates;

public record GetActionCardTemplatesQuery(string? CategoryCode = null) : IRequest<IReadOnlyList<ActionCardTemplateDto>>;
