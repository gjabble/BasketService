using BasketService.API.Domain;
using BasketService.API.Mapping;

namespace BasketService.API.DTO.Response;

public sealed record AddItemResponse
{
    public required BasketDto Basket { get; init; }

    public static AddItemResponse FromBasket(Basket basket) =>
        new() { Basket = BasketDtoMapper.FromBasket(basket) };
}