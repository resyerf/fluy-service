namespace Fluy.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    IReadOnlyCollection<string> Roles { get; }

    void SetUser(Guid userId, IEnumerable<string> roles);
}
