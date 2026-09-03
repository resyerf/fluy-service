using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Queries.Identity.GetUsers;

public class GetUsersQueryHandler(IUserRepository users) : IQueryHandler<GetUsersQuery, IReadOnlyCollection<UserDetail>>
{
    public Task<IReadOnlyCollection<UserDetail>> Handle(GetUsersQuery query, CancellationToken cancellationToken) =>
        users.GetAllWithRolesAsync(cancellationToken);
}
