using System;
using System.Collections.Generic;
using System.Linq;
using BasketService.API.Commands;
using BasketService.API.DTO.Request;
using Xunit;

namespace BasketService.Tests.Commands;

public sealed class AddItemCommandTests
{
    [Fact]
    public void ShouldCreateCommand_WhenMappingFromRequest_WithValidRequest()
    {
        var request = new AddItemRequest
        {
            ProductId = "HAT",
            Quantity = 2
        };

        var command = AddItemCommand.FromRequest(request);

        Assert.Equal("HAT", command.ProductId);
        Assert.Equal(2, command.Quantity);
    }

    [Fact]
    public void ShouldTrimProductId_WhenMappingFromRequest()
    {
        var request = new AddItemRequest
        {
            ProductId = "   HAT   ",
            Quantity = 1
        };

        var command = AddItemCommand.FromRequest(request);

        Assert.Equal("HAT", command.ProductId);
    }

    [Fact]
    public void ShouldThrowArgumentNullException_WhenMappingFromRequest_IfRequestIsNull()
    {
        AddItemRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => AddItemCommand.FromRequest(request!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldThrowArgumentException_WhenMappingFromRequest_IfProductIdIsInvalid(string? productId)
    {
        var request = new AddItemRequest
        {
            ProductId = productId!,
            Quantity = 1
        };

        var ex = Assert.Throws<ArgumentException>(() => AddItemCommand.FromRequest(request));
        Assert.Equal("Product Id is required. (Parameter 'request')", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ShouldThrowArgumentException_WhenMappingFromRequest_IfQuantityIsInvalid(int quantity)
    {
        var request = new AddItemRequest
        {
            ProductId = "HAT",
            Quantity = quantity
        };

        var ex = Assert.Throws<ArgumentException>(() => AddItemCommand.FromRequest(request));
        Assert.Equal("Quantity must be >= 1. (Parameter 'request')", ex.Message);
    }
}