using System.Net.Mail;
using Fluy.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Fluy.Infrastructure.External.Services;

/// <summary>
/// Adapter mínimo de CODE.md §4.17: `System.Net.Mail.SmtpClient` apuntando a un servidor SMTP
/// configurado (en desarrollo, un catcher local tipo MailHog/smtp4dev — CODE.md §9.22). No requiere
/// paquetes NuGet adicionales. El proveedor real de producción (SendGrid u otro) sigue sin decidirse
/// — este adapter es intercambiable porque el resto de la aplicación solo conoce <see cref="IEmailSender"/>.
/// </summary>
public class SmtpEmailSender(IOptions<SmtpSettings> options) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;

        using var client = new SmtpClient(settings.Host, settings.Port);
        using var message = new MailMessage
        {
            From = new MailAddress(settings.From, settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message, cancellationToken);
    }
}
