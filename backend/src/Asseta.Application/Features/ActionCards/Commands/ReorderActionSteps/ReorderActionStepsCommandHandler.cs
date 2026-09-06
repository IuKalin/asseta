using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.ReorderActionSteps;

public class ReorderActionStepsCommandHandler : IRequestHandler<ReorderActionStepsCommand, IReadOnlyList<ActionCardStepDto>>
{
    private readonly IAssetaDbContext _context;

    public ReorderActionStepsCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ActionCardStepDto>> Handle(ReorderActionStepsCommand request, CancellationToken cancellationToken)
    {
        var card = await _context.ActionCards
            .Include(c => c.Steps)
            .FirstOrDefaultAsync(c => c.Id == request.ActionCardId && !c.IsDeleted, cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(nameof(ActionCard), request.ActionCardId);
        }

        if (card.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        card.ReorderSteps(request.OrderedStepIds);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_STEPS_REORDERED",
            $"{{\"count\":{request.OrderedStepIds.Count}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return card.Steps
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.StepOrder)
            .Select(ActionCardStepDto.FromEntity)
            .ToList();
    }
}
