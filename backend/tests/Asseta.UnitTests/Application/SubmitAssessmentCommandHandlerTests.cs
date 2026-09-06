using Asseta.Application.Features.ContinuityMap.Commands.SubmitAssessment;
using Asseta.Application.Features.ContinuityMap.DTOs;
using Asseta.Application.Features.ContinuityMap.Queries.GetContinuityMap;
using Asseta.Domain.Enums;
using Asseta.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Asseta.UnitTests.Application;

public class SubmitAssessmentCommandHandlerTests
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
    public async Task SubmitAssessment_ShouldStoreHistoryAndCreateContinuityItems()
    {
        var context = CreateInMemoryContext();
        var ownerId = Guid.NewGuid();

        var mediatorMock = new FakeMediator(ownerId, context);

        var handler = new SubmitContinuityAssessmentCommandHandler(context, mediatorMock);

        var answers = new List<AssessmentAnswerDto>
        {
            new("q1", "FINANCIAL", "Tài khoản Vietcombank *1234", true, PriorityLevel.CRITICAL, "App VCB"),
            new("q2", "PROPERTY", "Căn hộ chung cư Vinhomes", true, PriorityLevel.IMPORTANT, "Két sắt"),
            new("q3", "INSURANCE", "Bảo hiểm Manulife", false) // hasItem = false
        };

        var result = await handler.Handle(
            new SubmitContinuityAssessmentCommand(ownerId, answers),
            CancellationToken.None);

        Assert.Equal(2, result.ItemsGeneratedCount);
        Assert.NotEqual(Guid.Empty, result.HistoryId);

        // Verify items created in DB
        var itemsInDb = await context.ContinuityItems.Where(i => i.OwnerId == ownerId).ToListAsync();
        Assert.Equal(2, itemsInDb.Count);

        // Verify history stored in DB
        var historyInDb = await context.ContinuityAssessmentHistories.FirstOrDefaultAsync(h => h.Id == result.HistoryId);
        Assert.NotNull(historyInDb);
        Assert.Equal(2, historyInDb.ItemsGeneratedCount);
    }

    private class FakeMediator : IMediator
    {
        private readonly Guid _ownerId;
        private readonly AssetaDbContext _context;

        public FakeMediator(Guid ownerId, AssetaDbContext context)
        {
            _ownerId = ownerId;
            _context = context;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request is GetContinuityMapQuery)
            {
                var queryHandler = new GetContinuityMapQueryHandler(_context);
                var res = await queryHandler.Handle(new GetContinuityMapQuery(_ownerId), cancellationToken);
                return (TResponse)(object)res;
            }
            throw new NotImplementedException();
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return Task.CompletedTask;
        }
    }
}
