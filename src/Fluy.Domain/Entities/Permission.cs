using Fluy.SharedKernel;

namespace Fluy.Domain.Entities;

/// <summary>
/// Catálogo global de permisos definidos por la plataforma FLUY. No es tenant-scoped:
/// el mismo catálogo de "qué se puede hacer" aplica a todos los tenants (CLAUDE.md §8).
/// </summary>
public class Permission : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Permission()
    {
    }

    public static Permission Create(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("El código del permiso es obligatorio.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La descripción del permiso es obligatoria.", nameof(description));
        }

        return new Permission
        {
            Code = code.Trim().ToLowerInvariant(),
            Description = description.Trim()
        };
    }
}
