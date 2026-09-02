using Fluy.Application.Common.Exceptions;
using Fluy.Application.Common.Interfaces;
using Fluy.Application.Common.Interfaces.Repositories;
using Fluy.Domain.Approvals;
using Fluy.Domain.Notifications;
using Fluy.Domain.Requests;
using Fluy.Domain.Workflows;
using Fluy.SharedKernel;
using Fluy.SharedKernel.Dispatching;

namespace Fluy.Application.Requests.SubmitRequest;

public class SubmitRequestCommandHandler(
    IRequestRepository requests,
    IApprovalRepository approvals,
    IWorkflowInstanceRepository workflowInstances,
    IWorkflowVersionRepository workflowVersions,
    INotificationRecipientResolver notificationRecipients,
    INotificationRepository notifications,
    IUnitOfWork unitOfWork,
    ICurrentTenantService currentTenant,
    IDateTime dateTime)
    : ICommandHandler<SubmitRequestCommand, SubmitRequestResult>
{
    public async Task<SubmitRequestResult> Handle(SubmitRequestCommand command, CancellationToken cancellationToken)
    {
        var tenantId = currentTenant.TenantId!.Value;

        var request = await requests.GetByIdAsync(command.RequestId, cancellationToken)
            ?? throw new RequestNotFoundException(command.RequestId);

        var now = dateTime.UtcNow;

        try
        {
            request.Submit(now);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidRequestStateException(ex.Message);
        }

        // Un reenvío tras corrección deja atrás cualquier instancia previa del workflow para esta
        // Request (CODE.md §9.20) — el reenvío siempre arranca desde el paso inicial, no retoma
        // donde quedó, igual que el Approval Engine mínimo reiniciaba siempre en Tier 1.
        var previousInstance = await workflowInstances.GetRunningForRequestAsync(request.Id, cancellationToken);
        previousInstance?.Cancel();

        var activeVersion = await workflowVersions.GetActivePublishedForTenantAsync(tenantId, cancellationToken);

        if (activeVersion is not null)
        {
            // Workflow Engine genérico (CLAUDE.md §14-16, CODE.md §9.20): el paso inicial de la
            // versión publicada del tenant decide el primer aprobador — reemplaza el Approval
            // Engine mínimo de un solo paso siempre-igual.
            var initialStep = await workflowVersions.GetStepAsync(activeVersion.InitialStepId!.Value, cancellationToken)
                ?? throw new WorkflowMisconfiguredException(activeVersion.InitialStepId.Value);

            var instance = WorkflowInstance.Start(
                tenantId, activeVersion.WorkflowDefinitionId, activeVersion.Id, request.Id, initialStep.Id);
            workflowInstances.Add(instance);

            approvals.Add(Approval.CreatePending(
                tenantId, request.Id, tier: 1, requiredRoleId: initialStep.ApproverRoleId,
                workflowInstanceId: instance.Id, workflowStepId: initialStep.Id));

            await NotifyApproversAsync(tenantId, request, initialStep.ApproverRoleId, cancellationToken);
        }
        else
        {
            // Fallback sin Workflow configurado: un único Approval Pending que cualquier usuario
            // con `request.approve` puede resolver (comportamiento previo a CODE.md §9.20).
            approvals.Add(Approval.CreatePending(tenantId, request.Id));

            await NotifyApproversAsync(tenantId, request, requiredRoleId: null, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitRequestResult(request.Id, request.Status.ToString(), now);
    }

    private async Task NotifyApproversAsync(
        Guid tenantId, Request request, Guid? requiredRoleId, CancellationToken cancellationToken)
    {
        var approverIds = await notificationRecipients.GetApproversAsync(requiredRoleId, cancellationToken);

        foreach (var approverId in approverIds)
        {
            notifications.Add(Notification.Create(
                tenantId, approverId, NotificationType.ApprovalAssigned,
                title: "Nueva solicitud pendiente de tu aprobación",
                message: request.Title,
                actorUserId: request.RequesterId,
                requestId: request.Id));
        }
    }
}
