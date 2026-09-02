using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.GetUsers;

public class GetUsersQueryHandler(IUserRepository users) : IQueryHandler<GetUsersQuery, IReadOnlyCollection<UserDetail>>
{
    public Task<IReadOnlyCollection<UserDetail>> Handle(GetUsersQuery query, CancellationToken cancellationToken) =>
        users.GetAllWithRolesAsync(cancellationToken);
}
