using FluentValidation;

namespace Fluy.Application.Identity.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.FullName).NotEmpty().MaximumLength(200);
    }
}
