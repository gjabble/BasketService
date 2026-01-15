using BasketService.API.Domain;
using BasketService.API.DTO.Response;

namespace BasketService.API.Mapping;

public static class BasketDtoMapper
{
    public static BasketDto FromBasket(Basket basket) => new()
    {
        BasketId = basket.Id.Value,
        Items = basket.Items.Select(i => new ItemDto
        {
            ItemId = i.Id.Value,
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList()
    };
}
