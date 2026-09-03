using Fluy.Application.Common.Exceptions;
using Fluy.Application.DTOs;
using Fluy.Application.Interfaces.Services;
using Fluy.Application.Interfaces.Repositories;
using Fluy.Domain.Entities;
using Fluy.Domain.Enums;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Commands.Approvals.ApproveRequest;

public class ApproveRequestCommandHandler(
    IApprovalRepository approvals,
    IRequestRepository requests,
    IWorkflowInstanceRepository workflowInstances,
    IWorkflowVersionRepository workflowVersions,
    INotificationRepository notifications,
    INotificationRecipientResolver notificationRecipients,
    IApprovalAuthorizationService approvalAuthorization,
    IAuditEventRepository auditEvents,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    ICurrentUserService currentUser,
    IDateTime dateTime)
    : ICommandHandler<ApproveRequestCommand, ApproveRequestResult>
{
    public async Task<ApproveRequestResult> Handle(ApproveRequestCommand command, CancellationToken cancellationToken)
    {
        var approval = await approvals.GetPendingByRequestIdAsync(command.RequestId, cancellationToken)
            ?? throw new ApprovalNotFoundException(command.RequestId);

        var request = await requests.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(command.RequestId);

        var userId = currentUser.UserId!.Value;
        await approvalAuthorization.EnsureCanDecideAsync(approval, userId, cancellationToken);

        var now = dateTime.UtcNow;
        var previousState = request.Status.ToString();
        approval.Approve(userId, command.Comment, now);

        if (approval.WorkflowInstanceId is not null)
        {
            // Workflow Engine genérico (CLAUDE.md §14-16, CODE.md §9.20): la transición de salida
            // del paso actual decide si hay un siguiente paso (con su propio aprobador) o si la
            // solicitud se completa — reemplaza el escalamiento hardcodeado a un tier 2 fijo.
            await AdvanceWorkflowAsync(approval, request, now, cancellationToken);
        }
        else
        {
            // Fallback sin Workflow configurado (CODE.md §9.20): un solo Approval siempre cierra.
            request.Complete();
            NotifyRequester(request, userId, NotificationType.RequestApproved, "Tu solicitud fue aprobada");
        }

        auditEvents.Add(AuditEvent.Create(
            request.TenantId, userId, "request.approved", nameof(Request), request.Id, now,
            previousState: previousState, newState: request.Status.ToString(), comment: command.Comment,
            ipAddress: currentUser.IpAddress, correlationId: currentUser.CorrelationId));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ApproveRequestResult(request.Id, request.Status.ToString());
    }

    private async Task AdvanceWorkflowAsync(
        Approval approval, Request request, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var instance = await workflowInstances.GetByIdAsync(approval.WorkflowInstanceId!.Value, cancellationToken)
            ?? throw new WorkflowMisconfiguredException(approval.WorkflowInstanceId.Value);

        var transitions = await workflowVersions.GetTransitionsFromStepAsync(approval.WorkflowStepId!.Value, cancellationToken);

        var match = transitions.FirstOrDefault(t => t.Matches(request.Amount))
            ?? throw new WorkflowMisconfiguredException(approval.WorkflowStepId!.Value);

        if (match.ToStepId is null)
        {
            instance.Complete(now);
            request.Complete();
            NotifyRequester(request, approval.ApproverId!.Value, NotificationType.RequestApproved, "Tu solicitud fue aprobada");
        }
        else
        {
            var nextStep = await workflowVersions.GetStepAsync(match.ToStepId.Value, cancellationToken)
                ?? throw new WorkflowMisconfiguredException(match.ToStepId.Value);
            instance.MoveTo(nextStep.Id);

            approvals.Add(Approval.CreatePending(
                tenantId, request.Id, tier: approval.Tier + 1, requiredRoleId: nextStep.ApproverRoleId,
                workflowInstanceId: instance.Id, workflowStepId: nextStep.Id));

            // La Request se queda en Submitted — todavía hay un paso pendiente.
            var nextApprovers = await notificationRecipients.GetApproversAsync(nextStep.ApproverRoleId, cancellationToken);
            foreach (var nextApproverId in nextApprovers)
            {
                notifications.Add(Notification.Create(
                    tenantId, nextApproverId, NotificationType.ApprovalAssigned,
                    title: "Nueva solicitud pendiente de tu aprobación",
                    message: request.Title,
                    actorUserId: approval.ApproverId, requestId: request.Id));
            }
        }
    }

    private void NotifyRequester(Request request, Guid actorUserId, NotificationType type, string title) =>
        notifications.Add(Notification.Create(
            request.TenantId, request.RequesterId, type, title, message: request.Title,
            actorUserId: actorUserId, requestId: request.Id));
}
