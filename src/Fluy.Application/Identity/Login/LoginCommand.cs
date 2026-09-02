using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Identity.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResult>;
