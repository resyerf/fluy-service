using FluentValidation;

namespace Fluy.Application.Commands.Approvals.RejectRequest;

public class RejectRequestCommandValidator : AbstractValidator<RejectRequestCommand>
{
    public RejectRequestCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
        RuleFor(c => c.Comment).NotEmpty().MaximumLength(2000).WithMessage("El motivo del rechazo es obligatorio.");
    }
}
