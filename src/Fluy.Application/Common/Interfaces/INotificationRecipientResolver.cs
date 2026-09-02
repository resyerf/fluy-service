namespace Fluy.Application.Common.Interfaces;

public interface INotificationRecipientResolver
{
    Task<IReadOnlyCollection<Guid>> GetApproversAsync(Guid? requiredRoleId, CancellationToken cancellationToken);
}
