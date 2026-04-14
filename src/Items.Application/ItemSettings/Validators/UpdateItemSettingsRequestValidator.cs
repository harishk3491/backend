using FluentValidation;
using Items.Application.ItemSettings.Dtos;

namespace Items.Application.ItemSettings.Validators;

public class UpdateItemSettingsRequestValidator : AbstractValidator<UpdateItemSettingsRequest>
{
    public UpdateItemSettingsRequestValidator()
    {
        RuleFor(x => x.ItemCodeMaxLength)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.AutoGenSequenceIncrement)
            .GreaterThan(0)
            .When(x => x.AutoGenSequenceIncrement.HasValue)
            .WithMessage("AutoGenSequenceIncrement must be greater than 0.");

        RuleFor(x => x.CcnNumberingBasis)
            .IsInEnum();
    }
}
