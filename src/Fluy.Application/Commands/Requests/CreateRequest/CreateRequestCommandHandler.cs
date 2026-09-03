using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Requests.CreateRequest;

public class CreateRequestCommandHandler(
    IRequestRepository requests,
    IBranchRepository branches,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    ICurrentUserService currentUser,
    IUsageTracker usageTracker,
    IDateTime dateTime) : ICommandHandler<CreateRequestCommand, CreateRequestResult>
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

        auditEvents.Add(AuditEvent.Create(
            tenantId, requesterId, "request.created", nameof(Request), request.Id, dateTime.UtcNow,
            newState: request.Status.ToString(), ipAddress: currentUser.IpAddress, correlationId: currentUser.CorrelationId));

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await usageTracker.IncrementAsync(tenantId, "max.requests.month", 1, cancellationToken);

        return new CreateRequestResult(request.Id, request.Status.ToString());
    }
}
