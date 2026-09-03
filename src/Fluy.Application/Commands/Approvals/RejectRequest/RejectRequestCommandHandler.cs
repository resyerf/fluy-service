using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.Domain.Enums;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Approvals.RejectRequest;

public class RejectRequestCommandHandler(
    IApprovalRepository approvals,
    IRequestRepository requests,
    IWorkflowInstanceRepository workflowInstances,
    INotificationRepository notifications,
    IApprovalAuthorizationService approvalAuthorization,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTime dateTime)
    : ICommandHandler<RejectRequestCommand, RejectRequestResult>
{
    public async Task<RejectRequestResult> Handle(RejectRequestCommand command, CancellationToken cancellationToken)
    {
        var approval = await approvals.GetPendingByRequestIdAsync(command.RequestId, cancellationToken)
            ?? throw new ApprovalNotFoundException(command.RequestId);

        var request = await requests.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(command.RequestId);

        var userId = currentUser.UserId!.Value;
        await approvalAuthorization.EnsureCanDecideAsync(approval, userId, cancellationToken);

        var now = dateTime.UtcNow;
        var previousState = request.Status.ToString();

        approval.Reject(userId, command.Comment, now);
        request.Reject();

        notifications.Add(Notification.Create(
            request.TenantId, request.RequesterId, NotificationType.RequestRejected,
            title: "Tu solicitud fue rechazada", message: command.Comment,
            actorUserId: userId, requestId: request.Id));

        auditEvents.Add(AuditEvent.Create(
            request.TenantId, userId, "request.rejected", nameof(Request), request.Id, now,
            previousState: previousState, newState: request.Status.ToString(), comment: command.Comment,
            ipAddress: currentUser.IpAddress, correlationId: currentUser.CorrelationId));

        if (approval.WorkflowInstanceId is not null)
        {
            var instance = await workflowInstances.GetByIdAsync(approval.WorkflowInstanceId.Value, cancellationToken)
                ?? throw new WorkflowMisconfiguredException(approval.WorkflowInstanceId.Value);
            instance.Cancel();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RejectRequestResult(request.Id, request.Status.ToString());
    }
}
