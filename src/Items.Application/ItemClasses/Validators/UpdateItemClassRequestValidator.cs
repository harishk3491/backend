using FluentValidation;
using Items.Application.ItemClasses.Dtos;

namespace Items.Application.ItemClasses.Validators;

public class UpdateItemClassRequestValidator : AbstractValidator<UpdateItemClassRequest>
{
    public UpdateItemClassRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.IssueMethod).IsInEnum();
        RuleFor(x => x.TraceabilityLevel).IsInEnum();

        RuleFor(x => x.AllowExcessGrnPercent)
            .InclusiveBetween(0, 100);

        RuleFor(x => x)
            .Must(x => x.Purchasable || x.Saleable)
            .WithName("Purchasable")
            .WithMessage("At least one of Purchasable or Saleable must be true.");

        RuleFor(x => x.FastMovingThreshold)
            .GreaterThan(x => x.SlowMovingThreshold)
            .WithMessage("FastMovingThreshold must be greater than SlowMovingThreshold.");

        // Inventory managed requires a valuation method
        When(x => x.InventoryManaged, () =>
        {
            RuleFor(x => x.ValuationMethod)
                .NotNull()
                .WithMessage("ValuationMethod is required when InventoryManaged is true.");
        });

        // TPI sub-fields
        When(x => x.TpiRequired, () =>
        {
            RuleFor(x => x.TpiTriggerStage)
                .NotNull()
                .WithMessage("TpiTriggerStage is required when TpiRequired is true.");
        });

        // Shelf life sub-fields
        When(x => x.ShelfLifeApplicable, () =>
        {
            RuleFor(x => x.AllowIssueIfExpired)
                .NotNull()
                .WithMessage("AllowIssueIfExpired is required when ShelfLifeApplicable is true.");
        });

        // Minimum production sub-fields
        When(x => x.MinimumProductionLogic, () =>
        {
            RuleFor(x => x.MinimumProductionQty)
                .NotNull().GreaterThan(0)
                .WithMessage("MinimumProductionQty must be greater than 0 when MinimumProductionLogic is true.");
            RuleFor(x => x.ProductionBatchMultiple)
                .NotNull().GreaterThan(0)
                .WithMessage("ProductionBatchMultiple must be greater than 0 when MinimumProductionLogic is true.");
        });

        // Auto allocation sub-field
        When(x => x.AutoAllocation, () =>
        {
            RuleFor(x => x.AutoAllocationOn)
                .NotNull()
                .WithMessage("AutoAllocationOn is required when AutoAllocation is true.");
        });
    }
}
