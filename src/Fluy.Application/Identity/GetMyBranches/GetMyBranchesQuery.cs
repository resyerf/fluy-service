using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetMyBranches;

/// <summary>
/// Sedes accesibles por el usuario actual (CLAUDE.md §7, CODE.md §9.25): si el usuario tiene al
/// menos un UserRole sin sede (alcance a todo el tenant), ve todas las sedes; si no, solo las
/// sedes donde tiene algún UserRole asignado. Sin permiso propio — cualquier usuario autenticado
/// puede consultar sus propias sedes, es la base del selector de sede post-login.
/// </summary>
public record GetMyBranchesQuery : IQuery<IReadOnlyCollection<MyBranchSummary>>;

public record MyBranchSummary(Guid Id, string Name);
