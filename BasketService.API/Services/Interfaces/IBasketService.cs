using BasketService.API.Commands;
using BasketService.API.Domain;
using BasketService.API.Domain.Outcomes;
using BasketService.API.Domain.ValueTypes;
using OneOf;


namespace BasketService.API.Services.Interfaces;

public interface IBasketService
{
    public Task<OneOf<Basket, DomainViolation>> CreateBasketAsync(CreateBasketCommand command, CancellationToken token = default);

    public Task<OneOf<Basket, NotFound, DomainViolation>> AddItemAsync(BasketId basketId, AddItemCommand command, CancellationToken token = default);

    public Task<OneOf<Basket, NotFound, DomainViolation>> AddItemsAsync(BasketId basketId, AddItemsCommand command, CancellationToken token = default);

    public Task<OneOf<Basket, NotFound, DomainViolation>> RemoveItemAsync(BasketId basketId, ItemId itemId, CancellationToken token = default);
}
