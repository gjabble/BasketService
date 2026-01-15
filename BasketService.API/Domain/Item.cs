using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.API.Domain;

public sealed class Item
{
    public ItemId Id { get; }
    public string ProductId { get; }
    public int Quantity { get; private set; }

    public Item(ItemId id, string productId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new DomainException("Product identifier is required.");
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");

        Id = id;
        ProductId = productId.Trim();
        Quantity = quantity;
    }

    public void Increment(int by)
    {
        if (by <= 0) throw new DomainException("Increment must be > 0.");
        checked { Quantity += by; }
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");
        Quantity = quantity;
    }
}
