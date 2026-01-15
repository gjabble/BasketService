using BasketService.API.Domain;
using BasketService.API.Mapping;

namespace BasketService.API.DTO.Response;

public sealed record RemoveItemResponse
{
    public required BasketDto Basket { get; init; }

    public static RemoveItemResponse FromBasket(Basket basket) =>
        new() { Basket = BasketDtoMapper.FromBasket(basket) };
}