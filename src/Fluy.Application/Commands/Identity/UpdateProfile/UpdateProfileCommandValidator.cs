using FluentValidation;

namespace Fluy.Application.Commands.Identity.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().MaximumLength(200);
    }
}
