using BasketService.API.Domain;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.Repository.Interfaces;
using System.Collections.Concurrent;

namespace BasketService.API.Repository.Implementations;

public sealed class InMemoryBasketStore : IBasketRepository
{
    private readonly ConcurrentDictionary<BasketId, Basket> _store;

    public InMemoryBasketStore()
    {
        _store = new ConcurrentDictionary<BasketId, Basket>();
    }

    public Task<Basket> CreateAsync(Basket basket, CancellationToken token = default)
    {
        if (!_store.TryAdd(basket.Id, basket))
            throw new InvalidOperationException($"Basket '{basket.Id}' already exists.");

        return Task.FromResult(basket);
    }

    public Task<Basket?> GetAsync(BasketId basketId, CancellationToken token = default)
    {
        _store.TryGetValue(basketId, out var basket);
        return Task.FromResult(basket);
    }

    public Task SaveAsync(Basket basket, CancellationToken token = default)
    {
        _store[basket.Id] = basket;
        return Task.CompletedTask;
    }
}
