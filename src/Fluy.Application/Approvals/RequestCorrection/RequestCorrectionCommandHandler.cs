using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Notifications;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Approvals.RequestCorrection;

public class RequestCorrectionCommandHandler(
    IApprovalRepository approvals,
    IRequestRepository requests,
    INotificationRepository notifications,
    IApprovalAuthorizationService approvalAuthorization,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTime dateTime)
    : ICommandHandler<RequestCorrectionCommand, RequestCorrectionResult>
{
    public async Task<RequestCorrectionResult> Handle(RequestCorrectionCommand command, CancellationToken cancellationToken)
    {
        var approval = await approvals.GetPendingByRequestIdAsync(command.RequestId, cancellationToken)
            ?? throw new ApprovalNotFoundException(command.RequestId);

        var request = await requests.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(command.RequestId);

        var userId = currentUser.UserId!.Value;
        await approvalAuthorization.EnsureCanDecideAsync(approval, userId, cancellationToken);

        var now = dateTime.UtcNow;

        approval.ReturnForCorrection(userId, command.Comment, now);
        request.ReturnForCorrection();

        notifications.Add(Notification.Create(
            request.TenantId, request.RequesterId, NotificationType.RequestReturnedForCorrection,
            title: "Tu solicitud requiere corrección", message: command.Comment,
            actorUserId: userId, requestId: request.Id));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RequestCorrectionResult(request.Id, request.Status.ToString());
    }
}
