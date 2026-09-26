using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Identity.UpdateProfile;

public class UpdateProfileCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : ICommandHandler<UpdateProfileCommand, UpdateProfileResult>
{
    public async Task<UpdateProfileResult> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var user = await users.GetByIdAsync(userId, cancellationToken)
            ?? throw new UserNotFoundException(userId);

        user.Rename(command.FullName);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResult(user.Id, user.Email, user.FullName);
    }
}
