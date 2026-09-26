using Fluy.Application.Common.Validation;
using FluentValidation;

namespace Fluy.Application.Commands.Identity.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.CurrentPassword).NotEmpty();
        RuleFor(c => c.NewPassword).MustBeAStrongPassword();
        RuleFor(c => c.NewPassword)
            .NotEqual(c => c.CurrentPassword)
            .WithMessage("La nueva contraseña debe ser diferente a la actual.");
    }
}
