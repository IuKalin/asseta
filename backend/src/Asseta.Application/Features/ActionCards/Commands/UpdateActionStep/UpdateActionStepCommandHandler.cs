using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.ActionCards.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.UpdateActionStep;

public class UpdateActionStepCommandHandler : IRequestHandler<UpdateActionStepCommand, ActionCardStepDto>
{
    private readonly IAssetaDbContext _context;

    public UpdateActionStepCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActionCardStepDto> Handle(UpdateActionStepCommand request, CancellationToken cancellationToken)
    {
        var step = await _context.ActionCardSteps
            .Include(s => s.ActionCard)
            .FirstOrDefaultAsync(s => s.Id == request.StepId && !s.IsDeleted, cancellationToken);

        if (step == null)
        {
            throw new NotFoundException(nameof(ActionCardStep), request.StepId);
        }

        if (step.ActionCard == null || step.ActionCard.IsDeleted)
        {
            throw new NotFoundException(nameof(ActionCard), step.ActionCardId);
        }

        if (step.ActionCard.OwnerId != request.OwnerId)
        {
            throw new UnauthorizedResourceAccessException();
        }

        step.Update(request.Instruction, request.EstimatedDuration);
        if (request.IsCompleted.HasValue)
        {
            step.ToggleCompleted(request.IsCompleted.Value);
        }

        step.ActionCard.EvaluateCompletion();

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            step.ActionCardId,
            "ACTION_STEP_UPDATED",
            $"{{\"stepId\":\"{step.Id}\",\"isCompleted\":{step.IsCompleted}}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return ActionCardStepDto.FromEntity(step);
    }
}
