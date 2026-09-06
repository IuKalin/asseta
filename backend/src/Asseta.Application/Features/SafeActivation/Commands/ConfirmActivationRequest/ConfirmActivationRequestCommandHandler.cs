using System.Text.Json;
using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.ConfirmActivationRequest;

public class ConfirmActivationRequestCommandHandler : IRequestHandler<ConfirmActivationRequestCommand, ActivationRequestDto>
{
    private readonly IAssetaDbContext _context;

    public ConfirmActivationRequestCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActivationRequestDto> Handle(ConfirmActivationRequestCommand request, CancellationToken cancellationToken)
    {
        var activationRequest = await _context.ActivationRequests
            .Include(r => r.Confirmations)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (activationRequest == null)
            throw new NotFoundException(nameof(ActivationRequest), request.RequestId);

        if (activationRequest.Status != ActivationRequestStatus.PendingGracePeriod)
            throw new InvalidOperationException("Chỉ có thể biểu quyết xác nhận yêu cầu kích hoạt đang trong thời gian đệm an toàn.");

        var trustedPerson = await _context.TrustedPeople
            .FirstOrDefaultAsync(p => p.OwnerId == activationRequest.OwnerId &&
                                      p.DelegateUserId == request.CurrentUserId &&
                                      p.Status == TrustedPersonStatus.Active &&
                                      !p.IsDeleted, cancellationToken);

        if (trustedPerson == null)
            throw new ForbiddenAccessException("Bạn không phải là Người Ủy Thác của chủ tài sản này.");

        if (trustedPerson.TrustLevel < 2)
            throw new ForbiddenAccessException("Người ủy thác Cấp 1 không có quyền tham gia biểu quyết kích hoạt.");

        var isExisting = activationRequest.Confirmations.Any(c => c.TrustedPersonId == trustedPerson.Id);
        var confirmation = activationRequest.AddConfirmation(trustedPerson.Id, request.IsConfirmed, request.Note);
        if (!isExisting)
        {
            _context.ActivationConfirmations.Add(confirmation);
        }

        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == activationRequest.OwnerId, cancellationToken);

        if (config == null)
        {
            config = OwnerActivationConfig.CreateDefault(activationRequest.OwnerId);
            _context.OwnerActivationConfigs.Add(config);
        }

        var minConfirmations = config.MinConfirmationsRequired;

        // If grace period has passed and confirmations met threshold -> activate
        if (activationRequest.GracePeriodExpiresAtUtc <= DateTime.UtcNow &&
            activationRequest.Confirmations.Count(c => c.IsConfirmed) >= minConfirmations)
        {
            activationRequest.ActivateEmergency();
            config?.SetStatus(HeartbeatStatus.Activated);
        }

        var auditLog = new ContinuityAuditLog(
            activationRequest.OwnerId,
            activationRequest.Id,
            "ACTIVATION_REQUEST_CONFIRMED",
            JsonSerializer.Serialize(new
            {
                activationRequest.Id,
                ConfirmedBy = trustedPerson.FullName,
                request.IsConfirmed,
                request.Note
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        var confirmationsList = activationRequest.Confirmations.Select(c => new ActivationConfirmationDto
        {
            Id = c.Id,
            TrustedPersonId = c.TrustedPersonId,
            TrustedPersonName = c.TrustedPersonId == trustedPerson.Id ? trustedPerson.FullName : "Người ủy thác",
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
            Reason = activationRequest.Reason,
            Status = activationRequest.Status.ToString(),
            GracePeriodExpiresAtUtc = activationRequest.GracePeriodExpiresAtUtc,
            RemainingSeconds = (int)Math.Max(0, (activationRequest.GracePeriodExpiresAtUtc - DateTime.UtcNow).TotalSeconds),
            ConfirmationsCount = activationRequest.Confirmations.Count(c => c.IsConfirmed),
            MinConfirmationsRequired = minConfirmations,
            RowVersion = activationRequest.RowVersion,
            Confirmations = confirmationsList
        };
    }
}
