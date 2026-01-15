using BasketService.API.Commands;
using BasketService.API.Domain;
using BasketService.API.Domain.Outcomes;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;
using BasketService.API.Repository.Interfaces;
using BasketService.API.Services.Interfaces;
using OneOf;

namespace BasketService.API.Services.Implementations;


public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<BasketService> _logger;

    public BasketService(IBasketRepository basketRepository, ILogger<BasketService> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    public async Task<OneOf<Basket, DomainViolation>> CreateBasketAsync(CreateBasketCommand command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        _logger.LogInformation("Creating basket");
        
        var basket = Basket.CreateNew();
        
        try
        {
            await _basketRepository.CreateAsync(basket, token);
            _logger.LogInformation("Created basket {BasketId}", basket.Id.Value);
            return basket;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Domain violation when adding item: BasketId={BasketId}",
                basket.Id.Value);

            return new DomainViolation(ex.Message);
        }
    }

    public async Task<OneOf<Basket, NotFound, DomainViolation>> AddItemAsync(BasketId basketId, AddItemCommand command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        _logger.LogInformation("Adding item: BasketId={BasketId} ProductId={ProductId} Qty={Qty}",
            basketId.Value, command.ProductId, command.Quantity);

        var basket = await _basketRepository.GetAsync(basketId, token);
        if (basket is null)
        {
            _logger.LogInformation("Basket not found: {BasketId}", basketId.Value);
            return new NotFound("Basket not found.");
        }

        try
        {
            basket.AddItem(command.ProductId, command.Quantity);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain violation when adding item: BasketId={BasketId} ProductId={ProductId}",
                basketId.Value, command.ProductId);

            return new DomainViolation(ex.Message);
        }

        await _basketRepository.SaveAsync(basket, token);

        _logger.LogInformation("Added item: BasketId={BasketId} ItemCount={Count}",
            basketId.Value, basket.Items.Count);

        return basket;
    }

    public async Task<OneOf<Basket, NotFound, DomainViolation>> AddItemsAsync(BasketId basketId, AddItemsCommand command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        _logger.LogInformation("Batch adding items: BasketId={BasketId} Count={Count}",
            basketId.Value, command.Items.Count);

        var basket = await _basketRepository.GetAsync(basketId, token);
        if (basket is null)
        {
            _logger.LogInformation("Basket not found: {BasketId}", basketId.Value);
            return new NotFound("Basket not found.");
        }

        try
        {
            foreach (var item in command.Items)
                basket.AddItem(item.ProductId, item.Quantity);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain violation during batch add: BasketId={BasketId}",
                basketId.Value);

            return new DomainViolation(ex.Message);
        }

        await _basketRepository.SaveAsync(basket, token);

        _logger.LogInformation("Batch add complete: BasketId={BasketId} ItemCount={Count}",
            basketId.Value, basket.Items.Count);

        return basket;
    }

    public async Task<OneOf<Basket, NotFound, DomainViolation>> RemoveItemAsync(BasketId basketId, ItemId itemId, CancellationToken token = default)
    {
        _logger.LogInformation("Removing item: BasketId={BasketId} ItemId={ItemId}",
            basketId.Value, itemId.Value);

        var basket = await _basketRepository.GetAsync(basketId, token);
        if (basket is null)
        {
            _logger.LogInformation("Basket not found: {BasketId}", basketId.Value);
            return new NotFound("Basket not found.");
        }

        try
        {
            basket.RemoveItem(itemId);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain violation when removing item: BasketId={BasketId} ItemId={ItemId}",
                basketId.Value, itemId.Value);

            return new DomainViolation(ex.Message);
        }
        catch (NotFoundException)
        {
            _logger.LogInformation("Item not found in basket: BasketId={BasketId} ItemId={ItemId}",
                basketId.Value, itemId.Value);

            return new NotFound("Item not found.");
        }

        await _basketRepository.SaveAsync(basket, token);

        _logger.LogInformation("Removed item: BasketId={BasketId} ItemCount={Count}",
            basketId.Value, basket.Items.Count);

        return basket;
    }
}
