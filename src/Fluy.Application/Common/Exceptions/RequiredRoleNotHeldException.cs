namespace Fluy.Application.Common.Exceptions;

public class RequiredRoleNotHeldException(string roleName)
    : Exception($"Este paso de aprobación requiere el rol '{roleName}', que el usuario actual no tiene.");
