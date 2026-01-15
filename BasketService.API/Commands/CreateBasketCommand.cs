using BasketService.API.DTO.Request;

namespace BasketService.API.Commands;

public sealed record CreateBasketCommand
{
    public static CreateBasketCommand FromRequest(CreateBasketRequest request)
        => new();
}