namespace Fluy.Application.Common.Exceptions;

public class EmailAlreadyRegisteredException(string email)
    : Exception($"Ya existe un usuario con el email '{email}' en este tenant.");
