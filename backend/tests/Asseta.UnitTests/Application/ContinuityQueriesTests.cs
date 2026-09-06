using Asseta.Application.Common.Exceptions;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityGaps;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityItemById;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;
using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application;

public class ContinuityQueriesTests
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
    public async Task GetContinuityMapQuery_ShouldReturnAllCategoriesAndCalculateScores()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        // Seed category and item
        var cat = await context.ContinuityCategories.FirstAsync(c => c.Code == "FINANCIAL");
        var item = new ContinuityItem(
            ownerId,
            cat.Id,
            "Khoản vay Vietcombank",
            PriorityLevel.CRITICAL,
            "Tủ hồ sơ",
            Guid.NewGuid());

        context.ContinuityItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new GetContinuityMapQueryHandler(context);
        var result = await handler.Handle(new GetContinuityMapQuery(ownerId), CancellationToken.None);

        Assert.Equal(6, result.Categories.Count);
        Assert.Equal(1, result.TotalItems);
        var finCat = result.Categories.First(c => c.Code == "FINANCIAL");
        Assert.Equal(100, finCat.ReadinessScore);
        Assert.Single(finCat.Items);
    }

    [Fact]
    public async Task GetContinuityGapsQuery_ShouldReturnUnassignedItems()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var cat = await context.ContinuityCategories.FirstAsync(c => c.Code == "PROPERTY");

        var gapItem = new ContinuityItem(
            ownerId,
            cat.Id,
            "Căn hộ Masteri",
            PriorityLevel.CRITICAL,
            documentLocationHint: null,
            assignedTrustedPersonId: null);

        context.ContinuityItems.Add(gapItem);
        await context.SaveChangesAsync();

        var handler = new GetContinuityGapsQueryHandler(context);
        var gaps = await handler.Handle(new GetContinuityGapsQuery(ownerId), CancellationToken.None);

        Assert.Single(gaps);
        Assert.Equal("Căn hộ Masteri", gaps[0].ItemName);
        Assert.Contains("AssignedTrustedPersonId", gaps[0].MissingFields);
        Assert.Contains("DocumentLocationHint", gaps[0].MissingFields);
    }

    [Fact]
    public async Task GetContinuityItemByIdQuery_WrongOwner_ShouldThrowUnauthorized()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();
        var otherUser = Guid.NewGuid();
        var cat = await context.ContinuityCategories.FirstAsync();

        var item = new ContinuityItem(ownerId, cat.Id, "Item A", PriorityLevel.LOW);
        context.ContinuityItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new GetContinuityItemByIdQueryHandler(context);
        await Assert.ThrowsAsync<UnauthorizedResourceAccessException>(() =>
            handler.Handle(new GetContinuityItemByIdQuery(item.Id, otherUser), CancellationToken.None));
    }
}
