namespace Fluy.Application.Common.Exceptions;

public class UserNotFoundException(Guid userId)
    : Exception($"No existe el usuario '{userId}' en este tenant.");
