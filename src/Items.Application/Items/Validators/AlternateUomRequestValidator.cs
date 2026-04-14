using FluentValidation;
using Items.Application.Items.Dtos;

namespace Items.Application.Items.Validators;

public class AlternateUomRequestValidator : AbstractValidator<CreateItemAlternateUomRequest>
{
    public AlternateUomRequestValidator()
    {
        RuleFor(x => x.UomId).GreaterThan(0).WithMessage("UomId must be a valid ID.");
        RuleFor(x => x.ConversionFactor).GreaterThan(0).WithMessage("ConversionFactor must be greater than 0.");
    }
}
