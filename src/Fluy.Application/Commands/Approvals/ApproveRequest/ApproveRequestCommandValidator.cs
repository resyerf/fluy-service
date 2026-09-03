using FluentValidation;

namespace Fluy.Application.Commands.Approvals.ApproveRequest;

public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
    }
}
