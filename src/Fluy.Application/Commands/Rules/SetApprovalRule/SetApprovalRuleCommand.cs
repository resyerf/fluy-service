using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Rules.SetApprovalRule;

/// <summary>
/// Upsert de la única `ApprovalRule` del tenant (CODE.md §9.19) — no hay un Command separado de
/// "crear" vs. "editar" porque, con una sola fila por tenant, la distinción no aporta nada.
/// </summary>
public record SetApprovalRuleCommand(decimal MinAmount, Guid SecondApproverRoleId)
    : ICommand<SetApprovalRuleResult>, IRequiresPermission
{
    public string PermissionCode => "rules.manage";
}
