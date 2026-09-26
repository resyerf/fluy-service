using Fluy.Application.Common.Validation;
using FluentValidation;

namespace Fluy.Application.Commands.Identity.SetPassword;

public class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordCommandValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
        RuleFor(c => c.NewPassword).MustBeAStrongPassword();
    }
}
