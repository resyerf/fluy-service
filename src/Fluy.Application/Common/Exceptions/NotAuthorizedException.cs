namespace Fluy.Application.Common.Exceptions;

public class NotAuthorizedException(string permissionCode)
    : Exception($"No tiene el permiso requerido: '{permissionCode}'.");
