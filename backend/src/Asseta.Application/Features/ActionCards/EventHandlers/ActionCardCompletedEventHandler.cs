using Asseta.Application.Common.Interfaces;
using Asseta.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asseta.Application.Features.ActionCards.EventHandlers;

public record ActionCardCompletedNotification(
    Guid CardId,
    Guid? ContinuityItemId,
    Guid CategoryId,
    Guid OwnerId) : INotification;

public class ActionCardCompletedEventHandler : INotificationHandler<ActionCardCompletedNotification>
{
    private readonly IAssetaDbContext _context;

    public ActionCardCompletedEventHandler(IAssetaDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActionCardCompletedNotification notification, CancellationToken cancellationToken)
    {
        await SyncCardCompletionAsync(notification.CardId, notification.OwnerId, cancellationToken);
    }

    public async Task SyncCardCompletionAsync(Guid cardId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var card = await _context.ActionCards
            .Include(c => c.Steps)
            .Include(c => c.ContinuityItem)
            .FirstOrDefaultAsync(c => c.Id == cardId && c.OwnerId == ownerId && !c.IsDeleted, cancellationToken);

        if (card == null || !card.ContinuityItemId.HasValue || card.ContinuityItem == null || card.ContinuityItem.IsDeleted)
        {
            return;
        }

        card.EvaluateCompletion();

        if (card.IsCompleted)
        {
            // Sync details to ContinuityItem to resolve gap
            var item = card.ContinuityItem;
            var updatedLocation = !string.IsNullOrWhiteSpace(card.DocumentLocationHint)
                ? card.DocumentLocationHint
                : item.DocumentLocationHint;

            var updatedPerson = card.AssignedTrustedPersonId ?? item.AssignedTrustedPersonId;

            item.UpdateDetails(
                item.Name,
                item.Priority,
                updatedLocation,
                updatedPerson);

            item.SetCompletionStatus(true);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
