using FluentValidation;

namespace Fluy.Application.Commands.Requests.SubmitRequest;

public class SubmitRequestCommandValidator : AbstractValidator<SubmitRequestCommand>
{
    public SubmitRequestCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
    }
}
