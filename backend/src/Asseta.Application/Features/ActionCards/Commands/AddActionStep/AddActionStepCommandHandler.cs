using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.AddActionStep;

public class AddActionStepCommandHandler : IRequestHandler<AddActionStepCommand, ActionCardStepDto>
{
    private readonly IAssetaDbContext _context;

    public AddActionStepCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardStepDto> Handle(AddActionStepCommand request, CancellationToken cancellationToken)
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

        var step = card.AddStep(request.Instruction, request.EstimatedDuration);
        _context.ActionCardSteps.Add(step);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            card.Id,
            "ACTION_STEP_ADDED",
            $"{{\"stepId\":\"{step.Id}\",\"stepOrder\":{step.StepOrder}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardStepDto.FromEntity(step);
    }
}
