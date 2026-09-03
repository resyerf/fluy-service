using FluentValidation;

namespace Fluy.Application.Commands.Requests.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Description).MaximumLength(4000);
        RuleFor(c => c.Amount).GreaterThanOrEqualTo(0).When(c => c.Amount.HasValue);

        RuleForEach(c => c.Fields).ChildRules(field =>
        {
            field.RuleFor(f => f.Key).NotEmpty().MaximumLength(100);
        });
    }
}
