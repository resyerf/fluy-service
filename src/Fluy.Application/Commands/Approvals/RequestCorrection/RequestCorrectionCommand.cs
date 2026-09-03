using Fluy.SharedKernel.Dispatching;
using Fluy.Application.DTOs;

namespace Fluy.Application.Commands.Approvals.RequestCorrection;

/// <summary>
/// CLAUDE.md no define un permiso distinto para "solicitar corrección" — se agrupa bajo
/// `request.approve` (es parte de la misma decisión de revisión que aprobar).
/// </summary>
public record RequestCorrectionCommand(Guid RequestId, string Comment) : ICommand<RequestCorrectionResult>, IRequiresPermission
{
    public string PermissionCode => "request.approve";
}
