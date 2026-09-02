using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Requests;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.CreateRequest;

public class CreateRequestCommandHandler(
    IRequestRepository requests,
    IBranchRepository branches,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    ICurrentUserService currentUser) : ICommandHandler<CreateRequestCommand, CreateRequestResult>
{
    public async Task<CreateRequestResult> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;
        var requesterId = currentUser.UserId!.Value;

        if (command.BranchId is { } branchId)
        {
            var branchExists = await branches.ExistsAsync(branchId, cancellationToken);
            if (!branchExists)
            {
                throw new BranchNotFoundException(branchId);
            }
        }

        var request = Request.Create(tenantId, requesterId, command.Title, command.Description, command.Amount, command.BranchId);
        requests.Add(request);

        if (command.Fields is not null)
        {
            requests.AddFields(command.Fields.Select(field => RequestField.Create(tenantId, request.Id, field.Key, field.Value)));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRequestResult(request.Id, request.Status.ToString());
    }
}
