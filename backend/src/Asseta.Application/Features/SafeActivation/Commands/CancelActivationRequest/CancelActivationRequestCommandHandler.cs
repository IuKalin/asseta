using System.Text.Json;
using Asseta.Application.Common.Exceptions;
using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.CancelActivationRequest;

public class CancelActivationRequestCommandHandler : IRequestHandler<CancelActivationRequestCommand, bool>
{
    private readonly IAssetaDbContext _context;

    public CancelActivationRequestCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CancelActivationRequestCommand request, CancellationToken cancellationToken)
    {
        var activationRequest = await _context.ActivationRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (activationRequest == null)
            throw new NotFoundException(nameof(ActivationRequest), request.RequestId);

        if (activationRequest.OwnerId != request.CurrentUserId)
            throw new ForbiddenAccessException("Chỉ Chủ tài sản mới có quyền hủy yêu cầu kích hoạt kế hoạch.");

        if (activationRequest.Status != ActivationRequestStatus.PendingGracePeriod)
            throw new InvalidOperationException("Chỉ có thể hủy yêu cầu kích hoạt đang trong thời gian đệm an toàn.");

        activationRequest.CancelByOwner();

        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == request.CurrentUserId, cancellationToken);

        if (config != null)
        {
            config.RecordCheckIn();
        }

        var auditLog = new ContinuityAuditLog(
            request.CurrentUserId,
            activationRequest.Id,
            "ACTIVATION_REQUEST_CANCELLED_BY_OWNER",
            JsonSerializer.Serialize(new
            {
                activationRequest.Id,
                CancelledAtUtc = DateTime.UtcNow
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
