using Fluy.Domain.Approvals;

namespace Fluy.Application.Common.Interfaces;

public interface IApprovalAuthorizationService
{
    Task EnsureCanDecideAsync(Approval approval, Guid userId, CancellationToken cancellationToken);
}
