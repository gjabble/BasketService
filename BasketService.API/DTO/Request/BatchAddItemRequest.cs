namespace BasketService.API.DTO.Request;

public sealed record BatchAddItemRequest
{
    public required List<AddItemRequest> Items { get; init; }
}