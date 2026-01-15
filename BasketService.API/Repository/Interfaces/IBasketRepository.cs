using BasketService.API.Domain;
using BasketService.API.Domain.ValueTypes;

namespace BasketService.API.Repository.Interfaces;

public interface IBasketRepository
{
    Task<Basket> CreateAsync(Basket basket, CancellationToken token = default);
    Task<Basket?> GetAsync(BasketId basketId, CancellationToken token = default);
    Task SaveAsync(Basket basket, CancellationToken token = default);
}
