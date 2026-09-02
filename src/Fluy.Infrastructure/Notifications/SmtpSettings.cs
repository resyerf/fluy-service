namespace Fluy.Infrastructure.Notifications;

public class SmtpSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public string From { get; set; } = "no-reply@fluy.local";
    public string FromName { get; set; } = "FLUY";
}
