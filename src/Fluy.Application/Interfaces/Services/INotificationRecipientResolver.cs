namespace Fluy.Application.Interfaces.Services;

public interface INotificationRecipientResolver
{
    Task<IReadOnlyCollection<Guid>> GetApproversAsync(Guid? requiredRoleId, CancellationToken cancellationToken);
}
