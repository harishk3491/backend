using FluentValidation;
using Items.Application.ItemVendorMappings.Dtos;

namespace Items.Application.ItemVendorMappings.Validators;

public class UpdateItemVendorMappingRequestValidator : AbstractValidator<UpdateItemVendorMappingRequest>
{
    public UpdateItemVendorMappingRequestValidator()
    {
        RuleFor(x => x.Moq).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StandardLeadTimeDays).GreaterThanOrEqualTo(0);

        RuleForEach(x => x.PurchaseUoms).SetValidator(new PurchaseUomRequestValidator());
        RuleForEach(x => x.Pricings).SetValidator(new VendorPricingRequestValidator());
    }
}
