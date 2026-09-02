namespace Fluy.Application.Common.Exceptions;

public class BranchNotFoundException(Guid branchId)
    : Exception($"No existe la sede '{branchId}' en este tenant.");
