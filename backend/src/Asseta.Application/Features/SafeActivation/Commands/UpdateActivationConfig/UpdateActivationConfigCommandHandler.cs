using System.Text.Json;
using Asseta.Application.Common.Interfaces;
using Asseta.Application.Features.SafeActivation.DTOs;
using Asseta.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.SafeActivation.Commands.UpdateActivationConfig;

public class UpdateActivationConfigCommandHandler : IRequestHandler<UpdateActivationConfigCommand, ActivationConfigDto>
{
    private readonly IAssetaDbContext _context;

    public UpdateActivationConfigCommandHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task<ActivationConfigDto> Handle(UpdateActivationConfigCommand request, CancellationToken cancellationToken)
    {
        var config = await _context.OwnerActivationConfigs
            .FirstOrDefaultAsync(c => c.OwnerId == request.OwnerId, cancellationToken);

        if (config == null)
        {
            config = new OwnerActivationConfig(
                request.OwnerId,
                request.CheckInIntervalDays,
                request.GracePeriodHours,
                request.MinConfirmationsRequired);
            _context.OwnerActivationConfigs.Add(config);
        }
        else
        {
            config.UpdateConfig(
                request.CheckInIntervalDays,
                request.GracePeriodHours,
                request.MinConfirmationsRequired);
        }

        var auditLog = new ContinuityAuditLog(
            request.OwnerId,
            null,
            "ACTIVATION_CONFIG_UPDATED",
            JsonSerializer.Serialize(new
            {
                request.CheckInIntervalDays,
                request.GracePeriodHours,
                request.MinConfirmationsRequired
            }),
            null,
            Guid.NewGuid());
        _context.ContinuityAuditLogs.Add(auditLog);

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivationConfigDto
        {
            CheckInIntervalDays = config.CheckInIntervalDays,
            GracePeriodHours = config.GracePeriodHours,
            MinConfirmationsRequired = config.MinConfirmationsRequired
        };
    }
}
