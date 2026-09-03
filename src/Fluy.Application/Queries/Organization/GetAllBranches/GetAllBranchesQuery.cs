using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Organization.GetAllBranches;

/// <summary>Listado plano de todas las sedes del tenant (con la empresa a la que pertenecen) — usado por el selector de "asignar rol a una sede" en /identity, CODE.md §9.25.</summary>
public record GetAllBranchesQuery : IQuery<IReadOnlyCollection<AllBranchDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}
