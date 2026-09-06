using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Queries.GetActionCardTemplates;

public class GetActionCardTemplatesQueryHandler : IRequestHandler<GetActionCardTemplatesQuery, IReadOnlyList<ActionCardTemplateDto>>
{
    private readonly IAssetaDbContext _context;

    public GetActionCardTemplatesQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ActionCardTemplateDto>> Handle(GetActionCardTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ActionCardTemplates
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.CategoryCode))
        {
            var code = request.CategoryCode.Trim().ToUpperInvariant();
            query = query.Where(t => t.CategoryCode == code);
        }

        var templates = await query
            .OrderBy(t => t.CategoryCode)
            .ThenBy(t => t.TitleVi)
            .ToListAsync(cancellationToken);

        return templates
            .Select(ActionCardTemplateDto.FromEntity)
            .ToList();
    }
}
