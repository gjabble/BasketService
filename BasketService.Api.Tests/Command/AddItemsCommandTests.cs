using BasketService.API.Commands;
using BasketService.API.DTO.Request;

namespace BasketService.Tests.Commands;

public sealed class AddItemsCommandTests
{
    [Fact]
    public void ShouldCreateCommand_WhenMappingFromRequest_WithValidItems()
    {
        var request = new BatchAddItemRequest
        {
            Items = new List<AddItemRequest>
            {
                new() { ProductId = "HAT", Quantity = 1 },
                new() { ProductId = "SCARF", Quantity = 2 }
            }
        };

        var command = AddItemsCommand.FromRequest(request);

        Assert.Equal(2, command.Items.Count);
        Assert.Equal("HAT", command.Items[0].ProductId);
        Assert.Equal(1, command.Items[0].Quantity);
        Assert.Equal("SCARF", command.Items[1].ProductId);
        Assert.Equal(2, command.Items[1].Quantity);
    }

    [Fact]
    public void ShouldThrowArgumentNullException_WhenMappingFromRequest_IfRequestIsNull()
    {
        BatchAddItemRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => AddItemsCommand.FromRequest(request!));
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenMappingFromRequest_IfItemsIsNull()
    {
        var request = new BatchAddItemRequest
        {
            Items = null!
        };

        var ex = Assert.Throws<ArgumentException>(() => AddItemsCommand.FromRequest(request));
        Assert.Equal("Items must contain at least one element. (Parameter 'request')", ex.Message);
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenMappingFromRequest_IfItemsIsEmpty()
    {
        var request = new BatchAddItemRequest
        {
            Items = new List<AddItemRequest>()
        };

        var ex = Assert.Throws<ArgumentException>(() => AddItemsCommand.FromRequest(request));
        Assert.Equal("Items must contain at least one element. (Parameter 'request')", ex.Message);
    }

    [Fact]
    public void ShouldThrowArgumentException_WhenMappingFromRequest_IfAnyItemIsInvalid()
    {
        var request = new BatchAddItemRequest
        {
            Items = new List<AddItemRequest>
            {
                new() { ProductId = "HAT", Quantity = 1 },
                new() { ProductId = "   ", Quantity = 2 }
            }
        };

        var ex = Assert.Throws<ArgumentException>(() => AddItemsCommand.FromRequest(request));
        Assert.Equal("Product Id is required. (Parameter 'request')", ex.Message);
    }

    [Fact]
    public void ShouldTrimProductId_ForEachItem_WhenMappingFromRequest()
    {
        var request = new BatchAddItemRequest
        {
            Items = new List<AddItemRequest>
            {
                new() { ProductId = "  HAT  ", Quantity = 1 },
                new() { ProductId = "SCARF", Quantity = 2 }
            }
        };

        var command = AddItemsCommand.FromRequest(request);

        Assert.Equal("HAT", command.Items[0].ProductId);
        Assert.Equal("SCARF", command.Items[1].ProductId);
    }
}