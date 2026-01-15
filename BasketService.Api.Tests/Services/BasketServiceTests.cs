using BasketService.API.Commands;
using BasketService.API.Domain;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BasketService.Api.Tests.Services;

public sealed class BasketServiceTests
{
    private readonly Mock<IBasketRepository> _repo = new();
    private readonly Mock<ILogger<BasketService.API.Services.Implementations.BasketService>> _logger = new();
    private readonly BasketService.API.Services.Implementations.BasketService _sut;

    public BasketServiceTests()
    {
        _sut = new BasketService.API.Services.Implementations.BasketService(_repo.Object, _logger.Object);
    }

    [Fact]
    public async Task ShouldCreateBasket_WhenCreatingBasket()
    {
        var result = await _sut.CreateBasketAsync(new CreateBasketCommand(), CancellationToken.None);

        Assert.True(result.IsT0);
        var basket = result.AsT0;

        Assert.NotEqual(Guid.Empty, basket.Id.Value);

        _repo.Verify(r => r.CreateAsync(
                It.Is<Basket>(b => b.Id.Value == basket.Id.Value),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnDomainViolation_WhenCreatingBasket_IfBasketAlreadyExists()
    {
        _repo.Setup(r => r.CreateAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Basket already exists."));

        var result = await _sut.CreateBasketAsync(new CreateBasketCommand(), CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Equal("Basket already exists.", result.AsT1.Message);

        _repo.Verify(r => r.CreateAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAddingItem_IfBasketDoesNotExist()
    {
        var basketId = BasketId.Create(Guid.NewGuid());

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(Basket));

        var cmd = new AddItemCommand(ProductId: "HAT", Quantity: 2);

        var result = await _sut.AddItemAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Equal("Basket not found.", result.AsT1.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAddItemAndSaveBasket_WhenAddingItem_WithValidInput()
    {
        var basket = Basket.CreateNew();
        var basketId = basket.Id;

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        _repo.Setup(r => r.SaveAsync(
                It.Is<Basket>(b => b.Id.Value == basketId.Value),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var cmd = new AddItemCommand(ProductId: "SCARF", Quantity: 2);

        var result = await _sut.AddItemAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT0);
        var updated = result.AsT0;

        Assert.Single(updated.Items);
        Assert.Equal("SCARF", updated.Items.Single().ProductId);
        Assert.Equal(2, updated.Items.Single().Quantity);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnDomainViolation_WhenAddingItem_IfDomainRuleIsViolated()
    {
        var basket = Basket.CreateNew();
        var basketId = basket.Id;

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // invalid product id
        var cmd = new AddItemCommand(ProductId: "   ", Quantity: 1);

        var result = await _sut.AddItemAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT2);
        Assert.Equal("Product identifier is required.", result.AsT2.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAddingItems_IfBasketDoesNotExist()
    {
        var basketId = BasketId.Create(Guid.NewGuid());

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Basket?)null);

        var cmd = new AddItemsCommand(new[]
        {
            new AddItemCommand("HAT", 1),
            new AddItemCommand("SCARF", 2)
        });

        var result = await _sut.AddItemsAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Equal("Basket not found.", result.AsT1.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldAddItemsAndSaveBasket_WhenAddingItems_WithValidInput()
    {
        var basket = Basket.CreateNew();
        var basketId = basket.Id;

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        _repo.Setup(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var cmd = new AddItemsCommand(new[]
        {
            new AddItemCommand("HAT", 1),
            new AddItemCommand("SCARF", 2)
        });

        var result = await _sut.AddItemsAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT0);
        var updated = result.AsT0;

        Assert.Equal(2, updated.Items.Count);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnDomainViolation_WhenAddingItems_IfAnyItemViolatesDomainRules()
    {
        var basket = Basket.CreateNew();
        var basketId = basket.Id;

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Second item invalid => domain exception
        var cmd = new AddItemsCommand(new[]
        {
            new AddItemCommand("HAT", 1),
            new AddItemCommand("   ", 2)
        });

        var result = await _sut.AddItemsAsync(basketId, cmd, CancellationToken.None);

        Assert.True(result.IsT2);
        Assert.Equal("Product identifier is required.", result.AsT2.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenRemovingItem_IfBasketDoesNotExist()
    {
        var basketId = BasketId.Create(Guid.NewGuid());
        var itemId = ItemId.New();

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Basket?)null);

        var result = await _sut.RemoveItemAsync(basketId, itemId, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Equal("Basket not found.", result.AsT1.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenRemovingItem_IfItemDoesNotExist()
    {
        var basket = Basket.CreateNew();
        var basketId = basket.Id;
        var missingItemId = ItemId.New();

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        var result = await _sut.RemoveItemAsync(basketId, missingItemId, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Equal("Item not found.", result.AsT1.Message);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRemoveItemAndSaveBasket_WhenRemovingItem_WithValidItem()
    {
        var basket = Basket.CreateNew();
        basket.AddItem("HAT", 1);
        basket.AddItem("SCARF", 1);

        var basketId = basket.Id;
        var toRemove = basket.Items.Single(i => i.ProductId == "HAT").Id;

        _repo.Setup(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        _repo.Setup(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.RemoveItemAsync(basketId, toRemove, CancellationToken.None);

        Assert.True(result.IsT0);
        var updated = result.AsT0;

        Assert.Single(updated.Items);
        Assert.Equal("SCARF", updated.Items.Single().ProductId);

        _repo.Verify(r => r.GetAsync(basketId, It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }
}
