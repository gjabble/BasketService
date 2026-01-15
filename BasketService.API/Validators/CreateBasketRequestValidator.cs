using BasketService.API.DTO.Request;

namespace BasketService.API.Validators;

using FluentValidation;

public sealed class CreateBasketRequestValidator : AbstractValidator<CreateBasketRequest>
{
    public CreateBasketRequestValidator()
    {
        // no rules yet, will need for VAT and cost requirements
    }
}
