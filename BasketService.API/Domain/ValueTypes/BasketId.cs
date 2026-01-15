using BasketService.API.Exceptions;

namespace BasketService.API.Domain.ValueTypes;

public readonly record struct BasketId(Guid Value)
{
    public static BasketId Create(Guid value)
        => value == Guid.Empty ? throw new DomainException("BasketId cannot be empty.") : new(value);
}
