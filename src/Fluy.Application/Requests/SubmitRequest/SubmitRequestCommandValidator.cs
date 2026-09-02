using FluentValidation;

namespace Fluy.Application.Requests.SubmitRequest;

public class SubmitRequestCommandValidator : AbstractValidator<SubmitRequestCommand>
{
    public SubmitRequestCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
    }
}
