namespace Fluy.Application.DTOs;

public record LoginResult(string Token, Guid UserId, string Email, string FullName, IReadOnlyCollection<string> Roles);
