using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.TrustedPeople.Commands.ClaimPairingCode;
using Asseta.Application.Features.TrustedPeople.Commands.CreateTrustedPerson;
using Asseta.Application.Features.TrustedPeople.Commands.RegeneratePairingCode;
using Asseta.Application.Features.TrustedPeople.Commands.RevokeTrustedPerson;
using Asseta.Application.Features.TrustedPeople.Commands.UpdateScopedPermissions;
using Asseta.Application.Features.TrustedPeople.Commands.UpdateTrustedPerson;
using Asseta.Application.Features.TrustedPeople.Queries.GetMyDelegatedRoles;
using Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPeople;
using Asseta.Application.Features.TrustedPeople.Queries.GetTrustedPersonDetail;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Asseta.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application.TrustedPeople;

public class TrustedPeopleHandlerTests
{
    private readonly PairingCodeHasher _hasher = new();

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
    public async Task CreateTrustedPerson_ValidCommand_PersistsPersonAndPairingCode()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var handler = new CreateTrustedPersonCommandHandler(context, _hasher);

        var command = new CreateTrustedPersonCommand(
            ownerId,
            "Nguyễn Văn B",
            "b@example.com",
            "0901234567",
            "Luật sư",
            2,
            "Phụ trách pháp lý");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Nguyễn Văn B", result.FullName);
        Assert.Equal("Invited", result.Status);
        Assert.NotNull(result.ActivePairingCode);
        Assert.Equal(6, result.ActivePairingCode.Length);
        Assert.NotNull(result.PairingExpiresAt);

        var inDb = await context.TrustedPeople
            .Include(p => p.PairingCodes)
            .FirstOrDefaultAsync(p => p.Id == result.Id);

        Assert.NotNull(inDb);
        Assert.Equal(ownerId, inDb.OwnerId);
        Assert.Single(inDb.PairingCodes);

        var auditInDb = await context.ContinuityAuditLogs.FirstOrDefaultAsync(a => a.ItemId == result.Id);
        Assert.NotNull(auditInDb);
        Assert.Equal("TRUSTED_PERSON_CREATED", auditInDb.Action);
    }

    [Fact]
    public async Task CreateTrustedPerson_Exceeding5People_ThrowsMaxTrustedPeopleExceededException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var handler = new CreateTrustedPersonCommandHandler(context, _hasher);

        for (int i = 1; i <= 5; i++)
        {
            var p = new TrustedPerson($"Person {i}", $"p{i}@example.com", $"090000000{i}", "Bạn", 1, ownerId);
            context.TrustedPeople.Add(p);
        }
        await context.SaveChangesAsync();

        var command = new CreateTrustedPersonCommand(
            ownerId,
            "Person 6",
            "p6@example.com",
            "0900000006",
            "Bạn",
            1);

        await Assert.ThrowsAsync<MaxTrustedPeopleExceededException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateTrustedPerson_DuplicateEmailOrPhone_ThrowsDuplicateContactException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var handler = new CreateTrustedPersonCommandHandler(context, _hasher);

        var existing = new TrustedPerson("Existing", "exist@example.com", "0901112233", "Vợ", 3, ownerId);
        context.TrustedPeople.Add(existing);
        await context.SaveChangesAsync();

        var dupEmailCommand = new CreateTrustedPersonCommand(ownerId, "New", "exist@example.com", "0999999999", "Anh", 1);
        await Assert.ThrowsAsync<DuplicateContactException>(() => handler.Handle(dupEmailCommand, CancellationToken.None));

        var dupPhoneCommand = new CreateTrustedPersonCommand(ownerId, "New2", "new2@example.com", "0901112233", "Em", 1);
        await Assert.ThrowsAsync<DuplicateContactException>(() => handler.Handle(dupPhoneCommand, CancellationToken.None));
    }

    [Fact]
    public async Task ClaimPairingCode_ValidCode_PairsSuccessfully()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var delegateUserId = Guid.NewGuid();

        // Tạo owner user
        var ownerUser = new User("owner@example.com", "hash", "Chủ tài sản A", "verifier", "salt");
        context.Users.Add(ownerUser);
        ownerId = ownerUser.Id;

        var person = new TrustedPerson("Luật sư B", "b@example.com", "0901234567", "Luật sư", 2, ownerId);
        var plainCode = _hasher.GeneratePairingCode();
        var (hash, salt) = _hasher.HashPairingCode(plainCode);
        var code = new TrustedPersonPairingCode(person.Id, hash, salt, DateTime.UtcNow.AddHours(48));
        person.AddPairingCode(code);

        context.TrustedPeople.Add(person);
        context.TrustedPersonPairingCodes.Add(code);
        await context.SaveChangesAsync();

        var handler = new ClaimPairingCodeCommandHandler(context, _hasher);
        var command = new ClaimPairingCodeCommand(delegateUserId, plainCode);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result.TrustedPersonId);
        Assert.Equal("Active", result.Status);
        Assert.Equal("Chủ tài sản A", result.OwnerDisplayName);

        var inDb = await context.TrustedPeople.FindAsync(person.Id);
        Assert.NotNull(inDb);
        Assert.Equal(TrustedPersonStatus.Active, inDb.Status);
        Assert.Equal(delegateUserId, inDb.DelegateUserId);

        var codeInDb = await context.TrustedPersonPairingCodes.FindAsync(code.Id);
        Assert.NotNull(codeInDb);
        Assert.True(codeInDb.IsUsed);
    }

    [Fact]
    public async Task ClaimPairingCode_SelfDelegation_ThrowsSelfDelegationProhibitedException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var person = new TrustedPerson("Me", "me@example.com", "0901234567", "Tôi", 1, ownerId);
        var plainCode = _hasher.GeneratePairingCode();
        var (hash, salt) = _hasher.HashPairingCode(plainCode);
        var code = new TrustedPersonPairingCode(person.Id, hash, salt, DateTime.UtcNow.AddHours(48));
        person.AddPairingCode(code);

        context.TrustedPeople.Add(person);
        context.TrustedPersonPairingCodes.Add(code);
        await context.SaveChangesAsync();

        var handler = new ClaimPairingCodeCommandHandler(context, _hasher);
        var command = new ClaimPairingCodeCommand(ownerId, plainCode); // DelegateUserId == OwnerId

        await Assert.ThrowsAsync<SelfDelegationProhibitedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateTrustedPerson_LowerTrustLevelTo1_AutoRemovesCategoryPermissions()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var person = new TrustedPerson("CFO", "cfo@example.com", "0903334455", "CFO", 2, ownerId);
        context.TrustedPeople.Add(person);

        var category = await context.ContinuityCategories.FirstAsync();
        var perm = TrustedPersonPermission.CreateForCategory(person.Id, category.Id);
        context.TrustedPersonPermissions.Add(perm);
        await context.SaveChangesAsync();

        var handler = new UpdateTrustedPersonCommandHandler(context);
        var command = new UpdateTrustedPersonCommand(
            person.Id,
            ownerId,
            "CFO Updated",
            "cfo@example.com",
            "0903334455",
            "CFO",
            1, // Hạ xuống Level 1 (Notice Only)
            1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, result.TrustLevel);

        var activePerms = await context.TrustedPersonPermissions
            .Where(p => p.TrustedPersonId == person.Id && !p.IsDeleted)
            .ToListAsync();

        Assert.Empty(activePerms);
    }

    [Fact]
    public async Task UpdateScopedPermissions_Level1Delegate_ThrowsValidationException()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var person = new TrustedPerson("Notice Only", "notice@example.com", "0905556677", "Bạn", 1, ownerId);
        context.TrustedPeople.Add(person);
        await context.SaveChangesAsync();

        var category = await context.ContinuityCategories.FirstAsync();
        var handler = new UpdateScopedPermissionsCommandHandler(context);
        var command = new UpdateScopedPermissionsCommand(
            person.Id,
            ownerId,
            new List<CategoryPermissionInput> { new(category.Id, true) });

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task RevokeTrustedPerson_UnassignsItemsAndCards()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var category = await context.ContinuityCategories.FirstAsync();

        var person = new TrustedPerson("Partner", "partner@example.com", "0907778899", "Cộng sự", 2, ownerId);
        context.TrustedPeople.Add(person);

        var item = new ContinuityItem(ownerId, category.Id, "Hợp đồng kinh doanh", PriorityLevel.CRITICAL, documentLocationHint: "Két sắt", assignedTrustedPersonId: person.Id);
        context.ContinuityItems.Add(item);

        var card = new ActionCard(Guid.NewGuid(), ownerId, category.Id, "Thẻ xử lý đối tác", UrgencyStage.IMMEDIATE, PriorityLevel.CRITICAL, assignedTrustedPersonId: person.Id);
        context.ActionCards.Add(card);

        await context.SaveChangesAsync();

        var handler = new RevokeTrustedPersonCommandHandler(context);
        var command = new RevokeTrustedPersonCommand(person.Id, ownerId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        var personInDb = await context.TrustedPeople.FindAsync(person.Id);
        Assert.NotNull(personInDb);
        Assert.True(personInDb.IsDeleted);
        Assert.Equal(TrustedPersonStatus.Revoked, personInDb.Status);

        var itemInDb = await context.ContinuityItems.FindAsync(item.Id);
        Assert.NotNull(itemInDb);
        Assert.Null(itemInDb.AssignedTrustedPersonId);
        Assert.True(itemInDb.HasContinuityGap); // Tự động kích hoạt lại Gap vì thiếu người phụ trách!

        var cardInDb = await context.ActionCards.FindAsync(card.Id);
        Assert.NotNull(cardInDb);
        Assert.Null(cardInDb.AssignedTrustedPersonId);
    }
}
