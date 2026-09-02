using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roles) : IQueryHandler<GetRolesQuery, IReadOnlyCollection<RoleDetail>>
{
    public Task<IReadOnlyCollection<RoleDetail>> Handle(GetRolesQuery query, CancellationToken cancellationToken) =>
        roles.GetAllWithPermissionsAsync(cancellationToken);
}
