using BasketService.API.Exceptions;

namespace BasketService.API.Domain.ValueTypes;

public readonly record struct ItemId(Guid Value)
{
    public static ItemId Create(Guid value)
        => value == Guid.Empty ? throw new DomainException("ItemId cannot be empty.") : new(value);

    public static ItemId New() => new(Guid.NewGuid());
}