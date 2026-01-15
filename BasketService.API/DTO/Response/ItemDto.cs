namespace BasketService.API.DTO.Response;

public sealed record ItemDto
{
    public required Guid ItemId { get; init; }
    public required string ProductId { get; init; }
    public required int Quantity { get; init; }
}