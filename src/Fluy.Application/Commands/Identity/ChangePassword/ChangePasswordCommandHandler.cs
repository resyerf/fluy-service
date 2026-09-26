using Fluy.Application.Common.Exceptions;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;
using Fluy.SharedKernel.Security;

namespace Fluy.Application.Commands.Identity.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IPasswordHasher passwordHasher) : ICommandHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var user = await users.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidCurrentPasswordException();
        }

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
