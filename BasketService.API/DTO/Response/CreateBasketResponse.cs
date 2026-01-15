using BasketService.API.Domain;
using BasketService.API.Mapping;

namespace BasketService.API.DTO.Response;

public sealed record CreateBasketResponse
{
    public required BasketDto Basket { get; init; }

    public static CreateBasketResponse FromBasket(Basket basket) =>
        new() { Basket = BasketDtoMapper.FromBasket(basket) };
}