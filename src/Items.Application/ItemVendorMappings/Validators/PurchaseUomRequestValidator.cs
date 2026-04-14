using FluentValidation;
using Items.Application.ItemVendorMappings.Dtos;

namespace Items.Application.ItemVendorMappings.Validators;

public class PurchaseUomRequestValidator : AbstractValidator<CreateItemVendorPurchaseUomRequest>
{
    public PurchaseUomRequestValidator()
    {
        RuleFor(x => x.PurchaseUomId).GreaterThan(0).WithMessage("PurchaseUomId must be a valid ID.");
        RuleFor(x => x.ConversionRate).GreaterThan(0).WithMessage("ConversionRate must be greater than 0.");
        RuleFor(x => x.TolerancePercent).GreaterThanOrEqualTo(0).WithMessage("TolerancePercent cannot be negative.");
    }
}
