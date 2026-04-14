using FluentValidation;
using Items.Application.PriceLists.Dtos;
using Items.Domain.Enums;

namespace Items.Application.PriceLists.Validators;

public class PriceListItemRequestValidator : AbstractValidator<CreatePriceListItemRequest>
{
    public PriceListItemRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.ItemId.HasValue || x.ItemClassId.HasValue)
            .WithName("ItemId")
            .WithMessage("Either ItemId or ItemClassId must be provided.");

        RuleFor(x => x.BasePrice).GreaterThan(0);

        When(x => x.DiscountType == DiscountType.Percent, () =>
        {
            RuleFor(x => x.DiscountValue)
                .Must(v => v.HasValue && v.Value >= 0 && v.Value < 100)
                .WithMessage("DiscountValue must be between 0 and 99.99 for Percent discount type.");
        });

        When(x => x.MinQty.HasValue && x.MaxQty.HasValue, () =>
        {
            RuleFor(x => x.MaxQty)
                .Must((req, max) => max >= req.MinQty)
                .WithMessage("MaxQty must be greater than or equal to MinQty.");
        });

        When(x => x.ValidFrom.HasValue && x.ValidTo.HasValue, () =>
        {
            RuleFor(x => x.ValidTo)
                .Must((req, to) => to > req.ValidFrom)
                .WithMessage("ValidTo must be after ValidFrom.");
        });
    }
}
