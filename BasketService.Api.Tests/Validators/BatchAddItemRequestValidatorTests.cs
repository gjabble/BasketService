using BasketService.API.DTO.Request;
using BasketService.API.Validators;

namespace BasketService.Api.Tests.Validators;

public sealed class BatchAddItemRequestValidatorTests
{
    private readonly BatchAddItemRequestValidator _validator = new();

    [Fact]
    public void ShouldBeValid_WhenBatchRequestIsValid()
    {
        var request = new BatchAddItemRequest
        {
            Items =
            [
                new AddItemRequest { ProductId = "P-001", Quantity = 1 },
                new AddItemRequest { ProductId = "P-002", Quantity = 2 }
            ]
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ShouldReturnError_WhenValidatingBatchRequest_IfItemsIsNull()
    {
        var request = new BatchAddItemRequest
        {
            Items = null!
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Items" && e.ErrorMessage == "Items collection is required.");
    }

    [Fact]
    public void ShouldReturnError_WhenValidatingBatchRequest_IfItemsIsEmpty()
    {
        var request = new BatchAddItemRequest
        {
            Items = []
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Items" && e.ErrorMessage == "At least one item is required.");
    }

    [Fact]
    public void ShouldReturnError_WhenValidatingBatchRequest_IfAnyItemHasEmptyProductId()
    {
        var request = new BatchAddItemRequest
        {
            Items =
            [
                new AddItemRequest { ProductId = "P-001", Quantity = 1 },
                new AddItemRequest { ProductId = "", Quantity = 1 }
            ]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Items[1].ProductId") && e.ErrorMessage == "ProductId is required.");
    }

    [Fact]
    public void ShouldReturnError_WhenValidatingBatchRequest_IfAnyItemHasInvalidQuantity()
    {
        var request = new BatchAddItemRequest
        {
            Items =
            [
                new AddItemRequest { ProductId = "P-001", Quantity = 1 },
                new AddItemRequest { ProductId = "P-002", Quantity = 0 }
            ]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Items[1].Quantity") && e.ErrorMessage == "Quantity must be greater than 0.");
    }
}