using BasketService.API.DTO.Request;

namespace BasketService.API.Commands;

public sealed record AddItemCommand(string ProductId, int Quantity)
{
    public static AddItemCommand FromRequest(AddItemRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var productId = request.ProductId?.Trim();
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("Product Id is required.", nameof(request));

        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be >= 1.", nameof(request));

        return new AddItemCommand(productId, request.Quantity);
    }
}   