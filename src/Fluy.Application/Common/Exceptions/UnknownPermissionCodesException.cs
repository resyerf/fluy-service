namespace Fluy.Application.Common.Exceptions;

public class UnknownPermissionCodesException(IReadOnlyCollection<string> codes)
    : Exception($"Los siguientes códigos de permiso no existen en el catálogo: {string.Join(", ", codes)}.");
