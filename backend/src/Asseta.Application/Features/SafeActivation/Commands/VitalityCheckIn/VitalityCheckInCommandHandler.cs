using System.Text.Json;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.VitalityCheckIn;

public class VitalityCheckInCommandHandler : IRequestHandler<VitalityCheckInCommand, ActivationStatusDto>
{
    private readonly IAssetaDbContext _context;

    public VitalityCheckInCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActivationStatusDto> Handle(VitalityCheckInCommand request, CancellationToken cancellationToken)
    {
        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == request.OwnerId, cancellationToken);

        if (config == null)
        {
            config = OwnerActivationConfig.CreateDefault(request.OwnerId);
            _context.OwnerActivationConfigs.Add(config);
        }

        config.RecordCheckIn();

        // If any activation request was running, cancel it immediately (Owner proved vitality)
        var pendingRequests = await _context.ActivationRequests
            .Where(r => r.OwnerId == request.OwnerId && r.Status == ActivationRequestStatus.PendingGracePeriod)
            .ToListAsync(cancellationToken);

        foreach (var req in pendingRequests)
        {
            req.CancelByOwner();
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            null,
            "VITALITY_CHECK_IN_RECORDED",
            JsonSerializer.Serialize(new
            {
                config.LastCheckInAtUtc,
                config.NextCheckInDueUtc,
                CancelledRequestsCount = pendingRequests.Count
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivationStatusDto
        {
            OwnerId = config.OwnerId,
            CheckInIntervalDays = config.CheckInIntervalDays,
            GracePeriodHours = config.GracePeriodHours,
            MinConfirmationsRequired = config.MinConfirmationsRequired,
            LastCheckInAtUtc = config.LastCheckInAtUtc,
            NextCheckInDueUtc = config.NextCheckInDueUtc,
            IsDueSoon = false,
            IsOverdue = false,
            HeartbeatStatus = "ACTIVE",
            IsEmergencyActive = false,
            ActiveRequest = null
        };
    }
}
