using FluentValidation;
using Items.Application.Items.Dtos;

namespace Items.Application.Items.Validators;

public class DrawingRequestValidator : AbstractValidator<CreateItemDrawingRequest>
{
    public DrawingRequestValidator()
    {
        RuleFor(x => x.DrawingName)
            .MaximumLength(100)
            .When(x => x.DrawingName != null);
    }
}
