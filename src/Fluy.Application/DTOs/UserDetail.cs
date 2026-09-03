namespace Fluy.Application.DTOs;

public record UserDetail(
    Guid Id,
    string Email,
    string FullName,
    string Status,
    IReadOnlyCollection<UserRoleDetail> Roles);
