namespace Fluy.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    IReadOnlyCollection<string> Roles { get; }
    string CorrelationId { get; }
    string? IpAddress { get; }

    void SetUser(Guid userId, IEnumerable<string> roles);
    void SetRequestContext(string correlationId, string? ipAddress);
}
