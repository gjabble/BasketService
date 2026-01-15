using System.Linq;
using BasketService.API.DTO.Request;
using BasketService.API.Validators;
using Xunit;

namespace BasketService.Tests.Validators;

public sealed class AddItemRequestValidatorTests
{
    private readonly AddItemRequestValidator _validator = new();

    [Fact]
    public void ShouldBeValid_WhenRequestIsValid()
    {
        var request = new AddItemRequest
        {
            ProductId = "P-001",
            Quantity = 1
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldReturnError_WhenValidatingRequest_IfProductIdIsEmpty(string productId)
    {
        var request = new AddItemRequest
        {
            ProductId = productId,
            Quantity = 1
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductId" && e.ErrorMessage == "ProductId is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void ShouldReturnError_WhenValidatingRequest_IfQuantityIsNotGreaterThanZero(int qty)
    {
        var request = new AddItemRequest
        {
            ProductId = "P-001",
            Quantity = qty
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity" && e.ErrorMessage == "Quantity must be greater than 0.");
    }
}


