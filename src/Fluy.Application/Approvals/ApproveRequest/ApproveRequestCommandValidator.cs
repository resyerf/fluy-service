using FluentValidation;

namespace Fluy.Application.Approvals.ApproveRequest;

public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
    }
}
