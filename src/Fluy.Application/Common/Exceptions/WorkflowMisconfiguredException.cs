namespace Fluy.Application.Common.Exceptions;

/// <summary>
/// Solo debería ocurrir si el grafo se corrompió después de publicarse (nunca en un flujo normal,
/// porque <c>WorkflowVersion.Publish</c> exige que todo paso tenga una transición que siempre
/// matchea) — se traduce a 500 dejándola caer, no a un código de error de negocio.
/// </summary>
public class WorkflowMisconfiguredException(Guid stepId)
    : Exception($"El paso de workflow '{stepId}' no tiene ninguna transición aplicable para esta solicitud.");
