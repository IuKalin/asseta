using System.Text.Json;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.DeactivateEmergencyPlan;

public class DeactivateEmergencyPlanCommandHandler : IRequestHandler<DeactivateEmergencyPlanCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public DeactivateEmergencyPlanCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeactivateEmergencyPlanCommand request, CancellationToken cancellationToken)
    {
        var activeRequests = await _context.ActivationRequests
            .Where(r => r.OwnerId == request.OwnerId &&
                        (r.Status == ActivationRequestStatus.Activated || r.Status == ActivationRequestStatus.PendingGracePeriod))
            .ToListAsync(cancellationToken);

        foreach (var req in activeRequests)
        {
            req.CancelByOwner();
        }

        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == request.OwnerId, cancellationToken);

        if (config != null)
        {
            config.RecordCheckIn();
            config.SetStatus(HeartbeatStatus.Active);
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            null,
            "PLAN_EMERGENCY_DEACTIVATED",
            JsonSerializer.Serialize(new
            {
                OwnerId = request.OwnerId,
                DeactivatedRequestsCount = activeRequests.Count,
                DeactivatedAtUtc = DateTime.UtcNow
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
