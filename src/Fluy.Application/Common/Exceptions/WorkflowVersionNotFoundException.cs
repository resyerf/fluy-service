namespace Fluy.Application.Common.Exceptions;

public class WorkflowVersionNotFoundException(Guid workflowVersionId)
    : Exception($"No existe la versión de workflow '{workflowVersionId}' en este tenant.");
