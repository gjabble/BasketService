using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.API.Domain;

public sealed class Basket
{
    private readonly List<Item> _items;
    public BasketId Id { get; }
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    private Basket(BasketId id)
    {
        Id = id;
        _items = [];
    }

    public static Basket CreateNew()
        => new(BasketId.Create(Guid.NewGuid()));

    public void AddItem(string productId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new DomainException("Product identifier is required.");
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");

        // if product already exists in basket, increment the quantity
        var existing = _items.FirstOrDefault(i => string.Equals(i.ProductId, productId.Trim(), StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            existing.Increment(quantity);
            return;
        }

        _items.Add(new Item(ItemId.New(), productId.Trim(), quantity));
    }

    public void RemoveItem(ItemId itemId)
    {
        var idx = _items.FindIndex(i => i.Id == itemId);
        if (idx < 0) throw new NotFoundException("Item not found.");
        _items.RemoveAt(idx);
    }
}
