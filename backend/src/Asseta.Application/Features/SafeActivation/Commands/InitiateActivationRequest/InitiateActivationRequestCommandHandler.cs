using System.Text.Json;
using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.InitiateActivationRequest;

public class InitiateActivationRequestCommandHandler : IRequestHandler<InitiateActivationRequestCommand, ActivationRequestDto>
{
    private readonly IAssetaDbContext _context;

    public InitiateActivationRequestCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActivationRequestDto> Handle(InitiateActivationRequestCommand request, CancellationToken cancellationToken)
    {
        TrustedPerson? initiator = null;

        // If not the owner self-testing, check if caller is an active Level 2 or 3 Trusted Person
        if (request.CurrentUserId != request.TargetOwnerId)
        {
            initiator = await _context.TrustedPeople
                .FirstOrDefaultAsync(p => p.OwnerId == request.TargetOwnerId &&
                                          p.DelegateUserId == request.CurrentUserId &&
                                          p.Status == TrustedPersonStatus.Active &&
                                          !p.IsDeleted, cancellationToken);

            if (initiator == null)
            {
                throw new ForbiddenAccessException("Bạn không phải là người ủy thác hợp lệ của tài khoản này.");
            }

            if (initiator.TrustLevel < 2)
            {
                throw new ForbiddenAccessException("Người ủy thác Cấp 1 (Chỉ nhận thông báo) không có quyền yêu cầu kích hoạt kế hoạch.");
            }
        }

        // Check if there is already an active request pending grace period
        var existingPending = await _context.ActivationRequests
            .AnyAsync(r => r.OwnerId == request.TargetOwnerId && r.Status == ActivationRequestStatus.PendingGracePeriod, cancellationToken);

        if (existingPending)
        {
            throw new ConflictException("Đang có một yêu cầu kích hoạt chờ xử lý cho tài khoản này.");
        }

        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == request.TargetOwnerId, cancellationToken);

        if (config == null)
        {
            config = OwnerActivationConfig.CreateDefault(request.TargetOwnerId);
            _context.OwnerActivationConfigs.Add(config);
        }

        var activationRequest = new ActivationRequest(
            request.TargetOwnerId,
            ActivationTriggerSource.TrustedPersonRequest,
            config.GracePeriodHours,
            initiator?.Id,
            request.Reason);

        _context.ActivationRequests.Add(activationRequest);
        config.SetStatus(HeartbeatStatus.PendingGracePeriod);

        var auditLog = new ContinuityAuditLog(
            request.TargetOwnerId,
            activationRequest.Id,
            "ACTIVATION_REQUEST_INITIATED",
            JsonSerializer.Serialize(new
            {
                activationRequest.Id,
                InitiatedBy = initiator?.FullName ?? "Chủ tài sản",
                request.Reason,
                activationRequest.GracePeriodExpiresAtUtc
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        var confirmationsList = activationRequest.Confirmations.Select(c => new ActivationConfirmationDto
        {
            Id = c.Id,
            TrustedPersonId = c.TrustedPersonId,
            TrustedPersonName = initiator?.FullName ?? "Người yêu cầu",
            IsConfirmed = c.IsConfirmed,
            Note = c.Note,
            ConfirmedAtUtc = c.ConfirmedAtUtc
        }).ToList();

        return new ActivationRequestDto
        {
            Id = activationRequest.Id,
            OwnerId = activationRequest.OwnerId,
            TriggerSource = activationRequest.TriggerSource.ToString(),
            InitiatedByTrustedPersonId = activationRequest.InitiatedByTrustedPersonId,
            InitiatedByTrustedPersonName = initiator?.FullName,
            Reason = activationRequest.Reason,
            Status = activationRequest.Status.ToString(),
            GracePeriodExpiresAtUtc = activationRequest.GracePeriodExpiresAtUtc,
            RemainingSeconds = (int)Math.Max(0, (activationRequest.GracePeriodExpiresAtUtc - DateTime.UtcNow).TotalSeconds),
            ConfirmationsCount = activationRequest.Confirmations.Count(c => c.IsConfirmed),
            MinConfirmationsRequired = config.MinConfirmationsRequired,
            RowVersion = activationRequest.RowVersion,
            Confirmations = confirmationsList
        };
    }
}
