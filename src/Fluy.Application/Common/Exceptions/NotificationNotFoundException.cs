namespace Fluy.Application.Common.Exceptions;

public class NotificationNotFoundException(Guid notificationId)
    : Exception($"No existe la notificación '{notificationId}' para este usuario.");
