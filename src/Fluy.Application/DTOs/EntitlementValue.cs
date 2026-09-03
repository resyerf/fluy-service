namespace Fluy.Application.DTOs;

/// <summary>
/// Lectura cacheada de platform.Subscriptions/platform.PlanFeatures (CODE.md §9.4, excepción
/// documentada #2). Devuelve la lista vacía si el tenant no tiene suscripción o está
/// Cancelled/Expired — nunca lanza, la ausencia de un feature en el resultado ES la respuesta
/// de "no lo tiene".
/// </summary>
public record EntitlementValue(string FeatureCode, string Value);
