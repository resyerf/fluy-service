namespace Fluy.Application.Common.Exceptions;

public class ApprovalNotFoundException(Guid requestId)
    : Exception($"No hay ninguna aprobación pendiente para la solicitud '{requestId}'.");
