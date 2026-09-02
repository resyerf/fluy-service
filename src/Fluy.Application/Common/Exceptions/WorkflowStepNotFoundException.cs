namespace Fluy.Application.Common.Exceptions;

public class WorkflowStepNotFoundException(Guid workflowStepId)
    : Exception($"No existe el paso de workflow '{workflowStepId}' en esta versión.");
