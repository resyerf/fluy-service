namespace Fluy.Application.Common.Exceptions;

public class CompanyNotFoundException(Guid companyId)
    : Exception($"No existe la empresa '{companyId}' en este tenant.");
