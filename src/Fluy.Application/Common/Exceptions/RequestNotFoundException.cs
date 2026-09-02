namespace Fluy.Application.Common.Exceptions;

public class RequestNotFoundException(Guid requestId)
    : Exception($"No existe la solicitud '{requestId}' en este tenant.");
