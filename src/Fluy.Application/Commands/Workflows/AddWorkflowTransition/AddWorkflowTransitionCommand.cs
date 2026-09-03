using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Workflows.AddWorkflowTransition;

/// <summary>
/// <see cref="ToStepId"/> nulo significa "completar la solicitud". <see cref="ConditionField"/>
/// nulo significa transición por defecto (sin condición) — hoy el único campo soportado es
/// "Amount" (CODE.md §9.20). <see cref="ConditionOperator"/> viaja como texto
/// (GreaterThanOrEqual/GreaterThan/LessThanOrEqual/LessThan/Equal) para no acoplar el contrato HTTP
/// al enum de dominio.
/// </summary>
public record AddWorkflowTransitionCommand(
    Guid WorkflowVersionId, Guid FromStepId, Guid? ToStepId,
    string? ConditionField, string? ConditionOperator, decimal? ConditionValue, int Order)
    : ICommand<AddWorkflowTransitionResult>, IRequiresPermission
{
    public string PermissionCode => "workflow.edit";
}
