using MediatR;

namespace Asseta.Application.Features.ContinuityMap.Commands.ReorderContinuityItems;

public record ReorderContinuityItemsCommand(
    Guid OwnerId,
    Guid CategoryId,
    List<Guid> OrderedItemIds) : IRequest<bool>;
