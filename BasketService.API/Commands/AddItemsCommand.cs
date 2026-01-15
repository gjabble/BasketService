using BasketService.API.DTO.Request;

namespace BasketService.API.Commands;

public sealed record AddItemsCommand(IReadOnlyList<AddItemCommand> Items)
{
    public static AddItemsCommand FromRequest(BatchAddItemRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("Items must contain at least one element.", nameof(request));

        var items = request.Items.Select(AddItemCommand.FromRequest).ToList();
        return new AddItemsCommand(items);
    }
}