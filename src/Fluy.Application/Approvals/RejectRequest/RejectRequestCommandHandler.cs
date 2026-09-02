using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Notifications;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Approvals.RejectRequest;

public class RejectRequestCommandHandler(
    IApprovalRepository approvals,
    IRequestRepository requests,
    IWorkflowInstanceRepository workflowInstances,
    INotificationRepository notifications,
    IApprovalAuthorizationService approvalAuthorization,
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

        approval.Reject(userId, command.Comment, now);
        request.Reject();

        notifications.Add(Notification.Create(
            request.TenantId, request.RequesterId, NotificationType.RequestRejected,
            title: "Tu solicitud fue rechazada", message: command.Comment,
            actorUserId: userId, requestId: request.Id));

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
