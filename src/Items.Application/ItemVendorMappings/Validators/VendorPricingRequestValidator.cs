using FluentValidation;
using Items.Application.ItemVendorMappings.Dtos;
using Items.Domain.Enums;

namespace Items.Application.ItemVendorMappings.Validators;

public class VendorPricingRequestValidator : AbstractValidator<CreateItemVendorPricingRequest>
{
    public VendorPricingRequestValidator()
    {
        RuleFor(x => x.BasePurchasePrice).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty();
        RuleFor(x => x.PaymentTerms).NotEmpty();

        When(x => x.DiscountType == DiscountType.Percent, () =>
        {
            RuleFor(x => x.DiscountValue)
                .Must(v => v.HasValue && v.Value >= 0 && v.Value <= 100)
                .WithMessage("DiscountValue must be between 0 and 100 for Percent discount type.");
        });

        When(x => x.PriceValidFrom.HasValue && x.PriceValidTo.HasValue, () =>
        {
            RuleFor(x => x.PriceValidTo)
                .Must((req, to) => to > req.PriceValidFrom)
                .WithMessage("PriceValidTo must be after PriceValidFrom.");
        });
    }
}
