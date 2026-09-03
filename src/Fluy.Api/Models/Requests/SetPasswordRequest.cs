namespace Fluy.Api.Models.Requests;

public record SetPasswordRequest(string Token, string NewPassword);
