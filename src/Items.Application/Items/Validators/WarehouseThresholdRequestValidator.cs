using FluentValidation;
using Items.Application.Items.Dtos;

namespace Items.Application.Items.Validators;

public class WarehouseThresholdRequestValidator : AbstractValidator<CreateItemWarehouseThresholdRequest>
{
    public WarehouseThresholdRequestValidator()
    {
        RuleFor(x => x.WarehouseName).NotEmpty();

        RuleFor(x => x.MinThreshold).GreaterThan(0);

        RuleFor(x => x.MaxThreshold)
            .Must((req, max) => max > req.MinThreshold)
            .WithMessage("MaxThreshold must be greater than MinThreshold.");

        RuleFor(x => x.ReorderQty).GreaterThan(0);

        RuleFor(x => x.OpeningRate)
            .Must(rate => rate.HasValue && rate.Value > 0)
            .When(x => x.OpeningQty.HasValue && x.OpeningQty > 0)
            .WithMessage("OpeningRate must be greater than 0 when OpeningQty is specified.");

        RuleFor(x => x.OpeningDate)
            .Must(d => d!.Value.Date <= DateTime.UtcNow.Date)
            .When(x => x.OpeningDate.HasValue)
            .WithMessage("OpeningDate cannot be a future date.");
    }
}
