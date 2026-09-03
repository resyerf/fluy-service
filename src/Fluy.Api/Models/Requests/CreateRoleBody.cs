namespace Fluy.Api.Models.Requests;

public record CreateRoleBody(string Name, IReadOnlyCollection<string> PermissionCodes);
