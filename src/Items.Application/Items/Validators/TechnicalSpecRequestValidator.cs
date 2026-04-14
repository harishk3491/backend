using FluentValidation;
using Items.Application.Items.Dtos;

namespace Items.Application.Items.Validators;

public class TechnicalSpecRequestValidator : AbstractValidator<CreateItemTechnicalSpecRequest>
{
    public TechnicalSpecRequestValidator()
    {
        RuleFor(x => x.CapabilityType).NotEmpty().WithMessage("CapabilityType is required.");
        RuleFor(x => x.CapabilityValue).NotEmpty().WithMessage("CapabilityValue is required.");
    }
}
