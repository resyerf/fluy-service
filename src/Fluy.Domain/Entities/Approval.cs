using Fluy.Domain.Common;
using Fluy.SharedKernel;
using Fluy.Domain.Enums;

namespace Fluy.Domain.Entities;

/// <summary>
/// Approval Engine (CLAUDE.md §17 — CODE.md §9.16/§9.19/§9.20): se crea un registro Pending cada vez
/// que una Request se envía o pasa a un siguiente paso. <see cref="WorkflowInstanceId"/>/
/// <see cref="WorkflowStepId"/> (nulos en filas creadas antes de CODE.md §9.20, o cuando el tenant
/// no tiene un Workflow publicado — fallback de un solo paso) enlazan la decisión con el motor de
/// Workflow genérico. <see cref="RequiredRoleId"/> null significa "cualquier usuario con
/// `request.approve` puede decidir"; con valor (tomado de <c>WorkflowStep.ApproverRoleId</c>, o del
/// ya superseded <c>ApprovalRule.SecondApproverRoleId</c> en filas viejas) significa "solo alguien
/// con ese rol puede decidir" — se valida en <c>ApprovalAuthorization</c>, no acá (el agregado no
/// conoce usuarios ni roles). ApproverId queda null hasta que alguien decide; ApprovalAction
/// (bitácora separada de acciones) queda fuera de este alcance mínimo — la decisión se registra
/// directamente acá (Status + Comment + DecidedAt + ApproverId).
/// </summary>
public class Approval : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid RequestId { get; private set; }
    public int Tier { get; private set; }
    public Guid? RequiredRoleId { get; private set; }
    public Guid? WorkflowInstanceId { get; private set; }
    public Guid? WorkflowStepId { get; private set; }
    public Guid? ApproverId { get; private set; }
    public ApprovalStatus Status { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private Approval()
    {
    }

    public static Approval CreatePending(
        Guid tenantId, Guid requestId, int tier = 1, Guid? requiredRoleId = null,
        Guid? workflowInstanceId = null, Guid? workflowStepId = null) => new()
        {
            TenantId = tenantId,
            RequestId = requestId,
            Tier = tier,
            RequiredRoleId = requiredRoleId,
            WorkflowInstanceId = workflowInstanceId,
            WorkflowStepId = workflowStepId,
            Status = ApprovalStatus.Pending
        };

    public void Approve(Guid approverId, string? comment, DateTimeOffset now) => Decide(ApprovalStatus.Approved, approverId, comment, now);

    public void Reject(Guid approverId, string comment, DateTimeOffset now) => Decide(ApprovalStatus.Rejected, approverId, comment, now);

    public void ReturnForCorrection(Guid approverId, string comment, DateTimeOffset now) =>
        Decide(ApprovalStatus.ReturnedForCorrection, approverId, comment, now);

    private void Decide(ApprovalStatus decision, Guid approverId, string? comment, DateTimeOffset now)
    {
        if (Status != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException($"Esta aprobación ya fue resuelta (estado actual: {Status}).");
        }

        Status = decision;
        ApproverId = approverId;
        Comment = comment;
        DecidedAt = now;
    }
}
