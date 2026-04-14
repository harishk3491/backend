using FluentValidation;
using Items.Application.ItemVendorMappings.Dtos;

namespace Items.Application.ItemVendorMappings.Validators;

public class CreateItemVendorMappingRequestValidator : AbstractValidator<CreateItemVendorMappingRequest>
{
    public CreateItemVendorMappingRequestValidator()
    {
        RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("ItemId must be a valid ID.");
        RuleFor(x => x.VendorId).GreaterThan(0).WithMessage("VendorId must be a valid ID.");
        RuleFor(x => x.Moq).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StandardLeadTimeDays).GreaterThanOrEqualTo(0);

        RuleForEach(x => x.PurchaseUoms).SetValidator(new PurchaseUomRequestValidator());
        RuleForEach(x => x.Pricings).SetValidator(new VendorPricingRequestValidator());
    }
}
