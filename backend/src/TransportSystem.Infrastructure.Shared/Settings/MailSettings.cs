namespace TransportSystem.Infrastructure.Shared.Settings
{
    public class MailSettings
    {
        public string DisplayName { get; init; }
        public string From { get; init; }
        public string SmtpHost { get; init; }
        public int SmtpPort { get; init; }
        public string SmtpUser { get; init; }
        public string SmtpPass { get; init; }
    }
}
