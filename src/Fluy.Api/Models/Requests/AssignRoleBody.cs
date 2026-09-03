namespace Fluy.Api.Models.Requests;

public record AssignRoleBody(Guid UserId, Guid RoleId, Guid? BranchId, Guid? DepartmentId);
