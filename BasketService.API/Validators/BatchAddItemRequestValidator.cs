using BasketService.API.DTO.Request;
using FluentValidation;

namespace BasketService.API.Validators;


public sealed class BatchAddItemRequestValidator : AbstractValidator<BatchAddItemRequest>
{
    public BatchAddItemRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Items collection is required.")
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new AddItemRequestValidator());
    }
}
