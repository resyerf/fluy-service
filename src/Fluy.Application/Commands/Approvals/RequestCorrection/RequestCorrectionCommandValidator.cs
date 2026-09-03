using FluentValidation;

namespace Fluy.Application.Commands.Approvals.RequestCorrection;

public class RequestCorrectionCommandValidator : AbstractValidator<RequestCorrectionCommand>
{
    public RequestCorrectionCommandValidator()
    {
        RuleFor(c => c.RequestId).NotEmpty();
        RuleFor(c => c.Comment).NotEmpty().MaximumLength(2000).WithMessage("El motivo de la corrección es obligatorio.");
    }
}
