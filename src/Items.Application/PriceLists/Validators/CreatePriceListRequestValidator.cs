using FluentValidation;
using Items.Application.PriceLists.Dtos;

namespace Items.Application.PriceLists.Validators;

public class CreatePriceListRequestValidator : AbstractValidator<CreatePriceListRequest>
{
    public CreatePriceListRequestValidator()
    {
        RuleFor(x => x.ApplicableTo).IsInEnum();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        // Note: SRS has a contradiction on Priority direction (grid says lower = higher, create screen says
        // higher = higher). Our implementation uses higher value = higher priority.
        RuleFor(x => x.Priority).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty();
        RuleFor(x => x.TargetType).IsInEnum();
        RuleFor(x => x.PricingScopeType).IsInEnum();

        RuleForEach(x => x.LineItems).SetValidator(new PriceListItemRequestValidator());
    }
}
