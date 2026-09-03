using Fluy.Application.Common.Exceptions;
using Fluy.Application.Commands.Notifications.ArchiveNotification;
using Fluy.Application.Queries.Notifications.GetMyNotifications;
using Fluy.Application.DTOs;
using Fluy.Application.Commands.Notifications.MarkAllNotificationsRead;
using Fluy.Application.Commands.Notifications.MarkNotificationRead;
using Fluy.SharedKernel.Dispatching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fluy.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<NotificationsResult>> GetMine(
        [FromQuery] NotificationFilter filter, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyNotificationsQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new MarkNotificationReadCommand(notificationId), cancellationToken);
            return NoContent();
        }
        catch (NotificationNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }

    [HttpPost("read-all")]
    public async Task<ActionResult<object>> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var count = await sender.Send(new MarkAllNotificationsReadCommand(), cancellationToken);
        return Ok(new { updated = count });
    }

    [HttpPost("{notificationId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid notificationId, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ArchiveNotificationCommand(notificationId), cancellationToken);
            return NoContent();
        }
        catch (NotificationNotFoundException ex)
        {
            return NotFound(new { detail = ex.Message });
        }
    }
}
