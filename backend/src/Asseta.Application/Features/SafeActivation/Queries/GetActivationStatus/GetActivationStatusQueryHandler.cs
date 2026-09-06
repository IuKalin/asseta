using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Queries.GetActivationStatus;

public class GetActivationStatusQueryHandler : IRequestHandler<GetActivationStatusQuery, ActivationStatusDto>
{
    private readonly IAssetaDbContext _context;

    public GetActivationStatusQueryHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActivationStatusDto> Handle(GetActivationStatusQuery request, CancellationToken cancellationToken)
    {
        var ownerId = request.TargetOwnerId ?? request.CurrentUserId;

        if (request.TargetOwnerId.HasValue && request.TargetOwnerId.Value != request.CurrentUserId)
        {
            var isPairedDelegate = await _context.TrustedPeople
                .AnyAsync(p => p.OwnerId == ownerId && p.DelegateUserId == request.CurrentUserId && p.Status == TrustedPersonStatus.Active && !p.IsDeleted, cancellationToken);

            if (!isPairedDelegate)
                throw new ForbiddenAccessException("Bạn không có quyền truy xuất thông tin kích hoạt của tài khoản này.");
        }

        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == ownerId, cancellationToken);

        if (config == null)
        {
            config = OwnerActivationConfig.CreateDefault(ownerId);
            _context.OwnerActivationConfigs.Add(config);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var activeRequest = await _context.ActivationRequests
            .Include(r => r.Confirmations)
            .Where(r => r.OwnerId == ownerId && (r.Status == ActivationRequestStatus.PendingGracePeriod || r.Status == ActivationRequestStatus.Activated))
            .OrderByDescending(r => r.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        // Check if grace period has expired and can transition to Activated
        if (activeRequest != null && activeRequest.Status == ActivationRequestStatus.PendingGracePeriod)
        {
            if (activeRequest.GracePeriodExpiresAtUtc <= DateTime.UtcNow)
            {
                if (activeRequest.Confirmations.Count(c => c.IsConfirmed) >= config.MinConfirmationsRequired)
                {
                    activeRequest.ActivateEmergency();
                    config.SetStatus(HeartbeatStatus.Activated);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        var now = DateTime.UtcNow;
        var isDueSoon = config.NextCheckInDueUtc <= now.AddDays(7) && config.NextCheckInDueUtc > now;
        var isOverdue = config.NextCheckInDueUtc <= now;

        var heartbeatStatus = config.Status.ToString().ToUpperInvariant();
        if (activeRequest != null && activeRequest.Status == ActivationRequestStatus.Activated)
        {
            heartbeatStatus = "ACTIVATED";
        }
        else if (activeRequest != null && activeRequest.Status == ActivationRequestStatus.PendingGracePeriod)
        {
            heartbeatStatus = "PENDING_GRACE_PERIOD";
        }
        else if (isOverdue)
        {
            heartbeatStatus = "WARNING";
        }

        ActivationRequestDto? requestDto = null;
        if (activeRequest != null)
        {
            string? initiatorName = null;
            if (activeRequest.InitiatedByTrustedPersonId.HasValue)
            {
                var initiator = await _context.TrustedPeople
                    .FirstOrDefaultAsync(p => p.Id == activeRequest.InitiatedByTrustedPersonId.Value, cancellationToken);
                initiatorName = initiator?.FullName;
            }

            var confirmationsList = new List<ActivationConfirmationDto>();
            foreach (var conf in activeRequest.Confirmations)
            {
                var tp = await _context.TrustedPeople
                    .FirstOrDefaultAsync(p => p.Id == conf.TrustedPersonId, cancellationToken);

                confirmationsList.Add(new ActivationConfirmationDto
                {
                    Id = conf.Id,
                    TrustedPersonId = conf.TrustedPersonId,
                    TrustedPersonName = tp?.FullName ?? "Người ủy thác",
                    IsConfirmed = conf.IsConfirmed,
                    Note = conf.Note,
                    ConfirmedAtUtc = conf.ConfirmedAtUtc
                });
            }

            requestDto = new ActivationRequestDto
            {
                Id = activeRequest.Id,
                OwnerId = activeRequest.OwnerId,
                TriggerSource = activeRequest.TriggerSource.ToString(),
                InitiatedByTrustedPersonId = activeRequest.InitiatedByTrustedPersonId,
                InitiatedByTrustedPersonName = initiatorName,
                Reason = activeRequest.Reason,
                Status = activeRequest.Status.ToString(),
                GracePeriodExpiresAtUtc = activeRequest.GracePeriodExpiresAtUtc,
                RemainingSeconds = (int)Math.Max(0, (activeRequest.GracePeriodExpiresAtUtc - DateTime.UtcNow).TotalSeconds),
                ConfirmationsCount = activeRequest.Confirmations.Count(c => c.IsConfirmed),
                MinConfirmationsRequired = config.MinConfirmationsRequired,
                RowVersion = activeRequest.RowVersion,
                Confirmations = confirmationsList
            };
        }

        return new ActivationStatusDto
        {
            OwnerId = config.OwnerId,
            CheckInIntervalDays = config.CheckInIntervalDays,
            GracePeriodHours = config.GracePeriodHours,
            MinConfirmationsRequired = config.MinConfirmationsRequired,
            LastCheckInAtUtc = config.LastCheckInAtUtc,
            NextCheckInDueUtc = config.NextCheckInDueUtc,
            IsDueSoon = isDueSoon,
            IsOverdue = isOverdue,
            HeartbeatStatus = heartbeatStatus,
            IsEmergencyActive = activeRequest?.Status == ActivationRequestStatus.Activated,
            ActiveRequest = requestDto
        };
    }
}
