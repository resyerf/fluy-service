namespace Fluy.Api.Models.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
