namespace BasketService.API.DTO.Request;

public sealed record AddItemRequest
{
    public required string ProductId { get; init; }
    public int Quantity { get; init; } = 1;
}