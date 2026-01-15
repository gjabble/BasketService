using BasketService.API.Domain.ValueTypes;
using BasketService.API.Exceptions;

namespace BasketService.Api.Tests.Domain;

public sealed class BasketIdTests
{
    [Fact]
    public void ShouldCreateBasketId_WhenGuidIsValid()
    {
        var guid = Guid.NewGuid();

        var basketId = BasketId.Create(guid);

        Assert.Equal(guid, basketId.Value);
    }

    [Fact]
    public void ShouldThrowDomainException_WhenGuidIsNotValid()
    {
        var ex = Assert.Throws<DomainException>(() => BasketId.Create(Guid.Empty));

        Assert.Equal("BasketId cannot be empty.", ex.Message);
    }
}