using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Identity.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResult>;
