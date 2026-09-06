using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.SafeActivation.Commands.CancelActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.ConfirmActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.DeactivateEmergencyPlan;
using Asseta.Application.Features.SafeActivation.Commands.InitiateActivationRequest;
using Asseta.Application.Features.SafeActivation.Commands.UpdateActivationConfig;
using Asseta.Application.Features.SafeActivation.Commands.VitalityCheckIn;
using Asseta.Application.Features.SafeActivation.Queries.GetActivationStatus;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application.SafeActivation;

public class SafeActivationHandlerTests
{
    private AssetaDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AssetaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AssetaDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetActivationStatus_NewOwner_AutoCreatesDefaultConfig()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var handler = new GetActivationStatusQueryHandler(context);
        var result = await handler.Handle(new GetActivationStatusQuery(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal(30, result.CheckInIntervalDays);
        Assert.Equal(48, result.GracePeriodHours);
        Assert.Equal("ACTIVE", result.HeartbeatStatus);
        Assert.False(result.IsEmergencyActive);
        Assert.Null(result.ActiveRequest);
    }

    [Fact]
    public async Task VitalityCheckIn_RecordsTimestampAndCancelsPendingRequests()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var config = OwnerActivationConfig.CreateDefault(ownerId);
        context.OwnerActivationConfigs.Add(config);

        var pendingReq = new ActivationRequest(ownerId, ActivationTriggerSource.SystemTimeout, 48);
        context.ActivationRequests.Add(pendingReq);
        await context.SaveChangesAsync();

        var handler = new VitalityCheckInCommandHandler(context);
        var result = await handler.Handle(new VitalityCheckInCommand(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("ACTIVE", result.HeartbeatStatus);

        var updatedReq = await context.ActivationRequests.FirstAsync(r => r.Id == pendingReq.Id);
        Assert.Equal(ActivationRequestStatus.CancelledByOwner, updatedReq.Status);
    }

    [Fact]
    public async Task UpdateActivationConfig_ValidParameters_UpdatesSettings()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var handler = new UpdateActivationConfigCommandHandler(context);
        var result = await handler.Handle(
            new UpdateActivationConfigCommand(ownerId, 60, 72, 2),
            CancellationToken.None);

        Assert.Equal(60, result.CheckInIntervalDays);
        Assert.Equal(72, result.GracePeriodHours);
        Assert.Equal(2, result.MinConfirmationsRequired);
    }

    [Fact]
    public async Task InitiateActivationRequest_Level1Delegate_ThrowsForbiddenAccessException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        var level1Delegate = new TrustedPerson(
            "Người nhận tin", "level1@test.com", "0900000001", "Bạn", 1, ownerId,
            status: TrustedPersonStatus.Active);
        level1Delegate.MarkAsPaired(delegateUserId);
        context.TrustedPeople.Add(level1Delegate);
        await context.SaveChangesAsync();

        var handler = new InitiateActivationRequestCommandHandler(context);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new InitiateActivationRequestCommand(delegateUserId, ownerId, "Thử kích hoạt"), CancellationToken.None));
    }

    [Fact]
    public async Task InitiateActivationRequest_Level2Delegate_InitiatesPendingGracePeriod()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        var level2Delegate = new TrustedPerson(
            "Người ủy thác 2", "level2@test.com", "0900000002", "Luật sư", 2, ownerId,
            status: TrustedPersonStatus.Active);
        level2Delegate.MarkAsPaired(delegateUserId);
        context.TrustedPeople.Add(level2Delegate);
        await context.SaveChangesAsync();

        var handler = new InitiateActivationRequestCommandHandler(context);
        var result = await handler.Handle(
            new InitiateActivationRequestCommand(delegateUserId, ownerId, "Chủ tài sản gặp nạn"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("PendingGracePeriod", result.Status);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Single(result.Confirmations);
        Assert.True(result.RemainingSeconds > 0);

        // Attempting to initiate second request while one is pending throws ConflictException
        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new InitiateActivationRequestCommand(delegateUserId, ownerId, "Kích hoạt lần 2"), CancellationToken.None));
    }

    [Fact]
    public async Task CancelActivationRequest_ByOwner_RevertsStatusToCancelled()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var req = new ActivationRequest(ownerId, ActivationTriggerSource.SystemTimeout, 48);
        context.ActivationRequests.Add(req);
        await context.SaveChangesAsync();

        var handler = new CancelActivationRequestCommandHandler(context);

        // Non-owner cannot cancel
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            handler.Handle(new CancelActivationRequestCommand(otherUserId, req.Id), CancellationToken.None));

        // Owner can cancel 1-tap
        var success = await handler.Handle(new CancelActivationRequestCommand(ownerId, req.Id), CancellationToken.None);
        Assert.True(success);

        var updated = await context.ActivationRequests.FirstAsync(r => r.Id == req.Id);
        Assert.Equal(ActivationRequestStatus.CancelledByOwner, updated.Status);
    }

    [Fact]
    public async Task ConfirmActivationRequest_VotesConfirmation_AndQuorumCheck()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var delegate1UserId = Guid.NewGuid();
        var delegate2UserId = Guid.NewGuid();

        var del1 = new TrustedPerson("Delegate 1", "d1@test.com", "0901", "Vợ", 3, ownerId, status: TrustedPersonStatus.Active);
        del1.MarkAsPaired(delegate1UserId);
        var del2 = new TrustedPerson("Delegate 2", "d2@test.com", "0902", "Luật sư", 2, ownerId, status: TrustedPersonStatus.Active);
        del2.MarkAsPaired(delegate2UserId);
        context.TrustedPeople.AddRange(del1, del2);

        var req = new ActivationRequest(ownerId, ActivationTriggerSource.TrustedPersonRequest, 48, del1.Id, "Khẩn cấp");
        context.ActivationRequests.Add(req);
        await context.SaveChangesAsync();

        var handler = new ConfirmActivationRequestCommandHandler(context);
        var result = await handler.Handle(
            new ConfirmActivationRequestCommand(delegate2UserId, req.Id, true, "Tôi cũng xác nhận"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.ConfirmationsCount);
    }

    [Fact]
    public async Task DeactivateEmergencyPlan_RollsBackToNormalState()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var config = OwnerActivationConfig.CreateDefault(ownerId);
        config.SetStatus(HeartbeatStatus.Activated);
        context.OwnerActivationConfigs.Add(config);

        var req = new ActivationRequest(ownerId, ActivationTriggerSource.SystemTimeout, 48);
        req.ActivateEmergency();
        context.ActivationRequests.Add(req);
        await context.SaveChangesAsync();

        var handler = new DeactivateEmergencyPlanCommandHandler(context);
        var result = await handler.Handle(new DeactivateEmergencyPlanCommand(ownerId), CancellationToken.None);

        Assert.True(result);
        var updatedConfig = await context.OwnerActivationConfigs.FirstAsync(c => c.OwnerId == ownerId);
        Assert.Equal(HeartbeatStatus.Active, updatedConfig.Status);
    }
}
