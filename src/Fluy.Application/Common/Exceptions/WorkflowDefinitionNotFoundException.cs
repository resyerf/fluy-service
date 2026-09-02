namespace Fluy.Application.Common.Exceptions;

public class WorkflowDefinitionNotFoundException(Guid workflowDefinitionId)
    : Exception($"No existe el workflow '{workflowDefinitionId}' en este tenant.");
