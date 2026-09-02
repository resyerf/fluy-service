using FluentValidation;

namespace Fluy.Application.Organization.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.LegalIdentifier).MaximumLength(50);
    }
}
