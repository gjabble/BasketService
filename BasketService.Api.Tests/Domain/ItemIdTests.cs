using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.Api.Tests.Domain;

public sealed class ItemIdTests
{
    [Fact]
    public void ShouldCreateItemId_WhenGuidIsValid()
    {
        var guid = Guid.NewGuid();

        var itemId = ItemId.Create(guid);

        Assert.Equal(guid, itemId.Value);
    }

    [Fact]
    public void ShouldThrowDomainException_WhenGuidIsEmpty()
    {
        var ex = Assert.Throws<DomainException>(() => ItemId.Create(Guid.Empty));

        Assert.Equal("ItemId cannot be empty.", ex.Message);
    }

    [Fact]
    public void ShouldCreateNonEmptyId_WhenGuidIsValid()
    {
        var itemId = ItemId.New();

        Assert.NotEqual(Guid.Empty, itemId.Value);
    }

    [Fact]
    public void ShouldCreateUniqueIds_WhenGuidIsValid()
    {
        var first = ItemId.New();
        var second = ItemId.New();

        Assert.NotEqual(first.Value, second.Value);
    }
}