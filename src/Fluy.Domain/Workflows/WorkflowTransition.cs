using Fluy.Domain.Common;
using Fluy.SharedKernel;

namespace Fluy.Domain.Workflows;

/// <summary>
/// Arista del grafo de un <see cref="WorkflowVersion"/>. <see cref="ToStepId"/> nulo significa
/// "completar la solicitud" (CLAUDE.md §14 — no existe un WorkflowStep especial "Fin"). Sin
/// condición (<see cref="ConditionField"/> nulo) es la transición por defecto de un paso; con
/// condición, generaliza el caso concreto de CLAUDE.md §16 ("¿Monto > 50000?") a cualquier campo
/// simple de la Request, absorbiendo lo que antes hacía <see cref="Fluy.Domain.Rules.ApprovalRule"/>
/// (CODE.md §9.19, ahora superseded por este motor).
/// </summary>
public class WorkflowTransition : AggregateRoot, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid WorkflowVersionId { get; private set; }
    public Guid FromStepId { get; private set; }
    public Guid? ToStepId { get; private set; }
    public string? ConditionField { get; private set; }
    public WorkflowConditionOperator? ConditionOperator { get; private set; }
    public decimal? ConditionValue { get; private set; }
    public int Order { get; private set; }

    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    private WorkflowTransition()
    {
    }

    public static WorkflowTransition Create(
        Guid tenantId, Guid workflowVersionId, Guid fromStepId, Guid? toStepId,
        string? conditionField, WorkflowConditionOperator? conditionOperator, decimal? conditionValue, int order)
    {
        if (fromStepId == toStepId)
        {
            throw new ArgumentException("Una transición no puede apuntar al mismo paso del que sale.", nameof(toStepId));
        }

        var hasCondition = !string.IsNullOrWhiteSpace(conditionField);
        if (hasCondition && (conditionOperator is null || conditionValue is null))
        {
            throw new ArgumentException("Una transición con condición necesita operador y valor.", nameof(conditionOperator));
        }

        if (!hasCondition && (conditionOperator is not null || conditionValue is not null))
        {
            throw new ArgumentException("Una transición sin campo de condición no puede tener operador/valor.", nameof(conditionField));
        }

        return new WorkflowTransition
        {
            TenantId = tenantId,
            WorkflowVersionId = workflowVersionId,
            FromStepId = fromStepId,
            ToStepId = toStepId,
            ConditionField = hasCondition ? conditionField!.Trim() : null,
            ConditionOperator = conditionOperator,
            ConditionValue = conditionValue,
            Order = order
        };
    }

    /// <summary>Único campo soportado hoy: "Amount" (CODE.md §9.20), comparado contra el monto de la Request.</summary>
    public bool Matches(decimal? amount)
    {
        if (ConditionField is null)
        {
            return true;
        }

        if (!string.Equals(ConditionField, "Amount", StringComparison.OrdinalIgnoreCase) || amount is null)
        {
            return false;
        }

        return ConditionOperator switch
        {
            WorkflowConditionOperator.GreaterThanOrEqual => amount >= ConditionValue,
            WorkflowConditionOperator.GreaterThan => amount > ConditionValue,
            WorkflowConditionOperator.LessThanOrEqual => amount <= ConditionValue,
            WorkflowConditionOperator.LessThan => amount < ConditionValue,
            WorkflowConditionOperator.Equal => amount == ConditionValue,
            _ => false
        };
    }
}
