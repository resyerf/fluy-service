using FluentValidation;

namespace Fluy.Application.Common.Validation;

/// <summary>
/// Requisitos de contraseña compartidos por SetPassword y ChangePassword — antes solo existía
/// MinimumLength(8), acá se agrega complejidad mínima (CLAUDE.md: "validar que la contraseña
/// cumpla con los requisitos").
/// </summary>
public static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> MustBeAStrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe incluir al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe incluir al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe incluir al menos un número.");
    }
}
