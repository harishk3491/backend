using FluentValidation;
using Items.Application.Items.Dtos;
using Items.Domain.Enums;

namespace Items.Application.Items.Validators;

public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        // Section 1: Identity
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Sku).NotEmpty();

        When(x => x.ParentItemId.HasValue, () =>
        {
            RuleFor(x => x.VariantName).NotEmpty().WithMessage("VariantName is required for variant items.");
            RuleFor(x => x.VariantCode).NotEmpty().WithMessage("VariantCode is required for variant items.");
        });

        // Section 4: Units & Physical
        RuleFor(x => x.BaseUomId).GreaterThan(0).WithMessage("BaseUomId must be a valid ID.");
        RuleFor(x => x.MaterialOfConstruction).NotEmpty();

        // Section 6: Tax & Regulatory
        RuleFor(x => x.HsnSacCode).NotEmpty();
        RuleFor(x => x.CountryOfOrigin).NotEmpty();
        RuleFor(x => x.TaxPreference).IsInEnum();

        // Customizable attributes
        RuleFor(x => x.AllowExcessGrnPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.IssueMethod).IsInEnum();
        RuleFor(x => x.TraceabilityLevel).IsInEnum();

        RuleFor(x => x)
            .Must(x => x.Purchasable || x.Saleable)
            .WithName("Purchasable")
            .WithMessage("At least one of Purchasable or Saleable must be true.");

        RuleFor(x => x.FastMovingThreshold)
            .GreaterThan(x => x.SlowMovingThreshold)
            .WithMessage("FastMovingThreshold must be greater than SlowMovingThreshold.");

        // ItemNature=Service cannot be inventory-managed
        When(x => x.ItemNature.HasValue && x.ItemNature == ItemNature.Service, () =>
        {
            RuleFor(x => x.InventoryManaged)
                .Must(v => !v)
                .WithMessage("Service items cannot be inventory-managed.");
        });

        // Inventory managed requires valuation method
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

        // Section 8: Tool details
        When(x => x.MaintenanceRequired == true, () =>
        {
            RuleFor(x => x.MaintenanceFrequencyValue)
                .NotNull().GreaterThan(0)
                .WithMessage("MaintenanceFrequencyValue is required when MaintenanceRequired is true.");
            RuleFor(x => x.MaintenanceFrequencyUnit)
                .NotNull()
                .WithMessage("MaintenanceFrequencyUnit is required when MaintenanceRequired is true.");
        });

        When(x => x.CalibrationRequired == true, () =>
        {
            RuleFor(x => x.CalibrationFrequencyValue)
                .NotNull().GreaterThan(0)
                .WithMessage("CalibrationFrequencyValue is required when CalibrationRequired is true.");
            RuleFor(x => x.CalibrationFrequencyUnit)
                .NotNull()
                .WithMessage("CalibrationFrequencyUnit is required when CalibrationRequired is true.");
        });

        // Sub-entities
        RuleForEach(x => x.AlternateUoms).SetValidator(new AlternateUomRequestValidator());
        RuleForEach(x => x.TechnicalSpecs).SetValidator(new TechnicalSpecRequestValidator());
        RuleForEach(x => x.Drawings).SetValidator(new DrawingRequestValidator());
        RuleForEach(x => x.WarehouseThresholds).SetValidator(new WarehouseThresholdRequestValidator());
    }
}
