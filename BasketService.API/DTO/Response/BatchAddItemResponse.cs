using BasketService.API.Domain;
using BasketService.API.Mapping;

namespace BasketService.API.DTO.Response;

public sealed record BatchAddItemResponse
{
    public required BasketDto Basket { get; init; }

    public static BatchAddItemResponse FromBasket(Basket basket) =>
        new() { Basket = BasketDtoMapper.FromBasket(basket) };
}