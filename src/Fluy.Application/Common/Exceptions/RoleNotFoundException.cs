namespace Fluy.Application.Common.Exceptions;

public class RoleNotFoundException(Guid roleId)
    : Exception($"No existe el rol '{roleId}' en este tenant.");
