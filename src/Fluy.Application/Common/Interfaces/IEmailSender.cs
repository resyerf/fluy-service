namespace Fluy.Application.Common.Interfaces;

/// <summary>Port de Notifications (CODE.md §4.17) — el adapter concreto vive en Infrastructure.</summary>
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
