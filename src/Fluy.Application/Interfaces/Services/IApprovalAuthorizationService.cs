using Fluy.Domain.Entities;

namespace Fluy.Application.Interfaces.Services;

public interface IApprovalAuthorizationService
{
    Task EnsureCanDecideAsync(Approval approval, Guid userId, CancellationToken cancellationToken);
}
