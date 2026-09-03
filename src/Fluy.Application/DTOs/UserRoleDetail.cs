namespace Fluy.Application.DTOs;

public record UserRoleDetail(Guid UserRoleId, Guid RoleId, string RoleName, Guid? BranchId, Guid? DepartmentId);
