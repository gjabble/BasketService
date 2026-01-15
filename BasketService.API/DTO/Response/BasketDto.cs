namespace BasketService.API.DTO.Response;

public sealed record BasketDto
{
    public required Guid BasketId { get; init; }
    public required List<ItemDto> Items { get; init; }
}