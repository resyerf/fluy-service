using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roles) : IQueryHandler<GetRolesQuery, IReadOnlyCollection<RoleDetail>>
{
    public Task<IReadOnlyCollection<RoleDetail>> Handle(GetRolesQuery query, CancellationToken cancellationToken) =>
        roles.GetAllWithPermissionsAsync(cancellationToken);
}
