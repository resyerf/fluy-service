namespace Fluy.Application.DTOs;

public record RoleDetail(Guid Id, string Name, bool IsSystemRole, IReadOnlyCollection<string> PermissionCodes);
