using FluentValidation;
using Items.Application.Uoms.Dtos;

namespace Items.Application.Uoms.Validators;

public class CreateUomRequestValidator : AbstractValidator<CreateUomRequest>
{
    public CreateUomRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
    }
}
