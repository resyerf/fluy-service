using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Organization.GetAllBranches;

/// <summary>Listado plano de todas las sedes del tenant (con la empresa a la que pertenecen) — usado por el selector de "asignar rol a una sede" en /identity, CODE.md §9.25.</summary>
public record GetAllBranchesQuery : IQuery<IReadOnlyCollection<AllBranchDetail>>, IRequiresPermission
{
    public string PermissionCode => "organization.manage";
}

public record AllBranchDetail(Guid Id, string Name, string CompanyName);
