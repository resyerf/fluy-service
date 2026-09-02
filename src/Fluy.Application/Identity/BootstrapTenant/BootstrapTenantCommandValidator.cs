using FluentValidation;

namespace Fluy.Application.Identity.BootstrapTenant;

public class BootstrapTenantCommandValidator : AbstractValidator<BootstrapTenantCommand>
{
    public BootstrapTenantCommandValidator()
    {
        RuleFor(c => c.MasterEmail).NotEmpty().EmailAddress();
        RuleFor(c => c.MasterFullName).NotEmpty().MaximumLength(200);
    }
}
