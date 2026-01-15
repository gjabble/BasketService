using BasketService.API.Domain;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.Api.Tests.Domain;

public sealed class BasketTests
{
    [Fact]
    public void ShouldCreateEmptyBasket_WithNonEmptyId()
    {
        var basket = Basket.CreateNew();

        Assert.NotEqual(Guid.Empty, basket.Id.Value);
        Assert.Empty(basket.Items);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldThrowDomainException_WhenAddingInvalidItemToBasket(string? productId)
    {
        var basket = Basket.CreateNew();

        var ex = Assert.Throws<DomainException>(() => basket.AddItem(productId!, 1));
        Assert.Equal("Product identifier is required.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void ShouldThrowDomainException_WhenAddingItemWithInvalidQuantity(int quantity)
    {
        var basket = Basket.CreateNew();

        var ex = Assert.Throws<DomainException>(() => basket.AddItem("HAT", quantity));
        Assert.Equal("Quantity must be > 0.", ex.Message);
    }

    [Fact]
    public void ShouldAddNewItemToBasket_WhenAddingValidItemToBasket()
    {
        var basket = Basket.CreateNew();

        basket.AddItem("HAT", 2);
        Assert.Single(basket.Items);
        
        var item = basket.Items.Single();
        Assert.NotEqual(Guid.Empty, item.Id.Value);
        Assert.Equal("HAT", item.ProductId);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void ShouldTrimProductId_WhenAddingValidItemToBasket()
    {
        var basket = Basket.CreateNew();

        basket.AddItem("  HAT  ", 1);

        var item = basket.Items.Single();
        Assert.Equal("HAT", item.ProductId);
    }

    [Fact]
    public void ShouldAddCorrectQuantityToBasket_WhenAddingMoreOfExistingItemToBasket()
    {
        var basket = Basket.CreateNew();

        basket.AddItem("HAT", 2);
        basket.AddItem("HAT", 3);

        Assert.Single(basket.Items);
        var item = basket.Items.Single();
        Assert.Equal("HAT", item.ProductId);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void ShouldRemoveItem_WhenRemovingValidItemFromBasket()
    {
        var basket = Basket.CreateNew();
        basket.AddItem("HAT", 1);
        basket.AddItem("SCARF", 1);

        var toRemove = basket.Items.Single(i => i.ProductId == "HAT").Id;

        basket.RemoveItem(toRemove);

        Assert.Single(basket.Items);
        Assert.Equal("SCARF", basket.Items.Single().ProductId);
    }

    [Fact]
    public void ShouldThrowNotFoundException_WhenRemovingNonExistentItemFromBasket()
    {
        var basket = Basket.CreateNew();

        var ex = Assert.Throws<NotFoundException>(() => basket.RemoveItem(ItemId.New()));
        Assert.Equal("Item not found.", ex.Message);
    }
}


