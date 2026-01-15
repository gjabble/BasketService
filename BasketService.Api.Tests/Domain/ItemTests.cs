using BasketService.API.Domain;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.Api.Tests.Domain;

public sealed class ItemTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenProductIdInvalid(string? productId)
    {
        var ex = Assert.Throws<DomainException>(() => new Item(ItemId.New(), productId!, 1));
        Assert.Equal("Product identifier is required.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenQuantityInvalid(int quantity)
    {
        var ex = Assert.Throws<DomainException>(() => new Item(ItemId.New(), "P-001", quantity));
        Assert.Equal("Quantity must be > 0.", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldTrimProductId()
    {
        var item = new Item(ItemId.New(), "  P-001  ", 2);

        Assert.Equal("P-001", item.ProductId);
        Assert.Equal(2, item.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Increment_ShouldThrow_WhenByInvalid(int by)
    {
        var item = new Item(ItemId.New(), "P-001", 1);

        var ex = Assert.Throws<DomainException>(() => item.Increment(by));
        Assert.Equal("Increment must be > 0.", ex.Message);
    }

    [Fact]
    public void Increment_ShouldIncreaseQuantity()
    {
        var item = new Item(ItemId.New(), "P-001", 2);

        item.Increment(3);

        Assert.Equal(5, item.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetQuantity_ShouldThrow_WhenQuantityInvalid(int quantity)
    {
        var item = new Item(ItemId.New(), "P-001", 1);

        var ex = Assert.Throws<DomainException>(() => item.SetQuantity(quantity));
        Assert.Equal("Quantity must be > 0.", ex.Message);
    }

    [Fact]
    public void SetQuantity_ShouldSetQuantity()
    {
        var item = new Item(ItemId.New(), "P-001", 1);

        item.SetQuantity(7);

        Assert.Equal(7, item.Quantity);
    }
}