using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.Commands.DeleteActionStep;

public class DeleteActionStepCommandHandler : IRequestHandler<DeleteActionStepCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public DeleteActionStepCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteActionStepCommand request, CancellationToken cancellationToken)
    {
        var step = await _context.ActionCardSteps
            .Include(s => s.ActionCard)
            .ThenInclude(c => c!.Steps)
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

        step.ActionCard.RemoveStep(step.Id);

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            step.ActionCardId,
            "ACTION_STEP_DELETED",
            $"{{\"stepId\":\"{step.Id}\"}}",
            null,
            Guid.NewGuid());

        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
