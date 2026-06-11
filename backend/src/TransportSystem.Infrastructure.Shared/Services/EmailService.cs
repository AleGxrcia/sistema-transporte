using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TransportSystem.Core.Application.Common.Dtos;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Infrastructure.Shared.Settings;

namespace TransportSystem.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<MailSettings> _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = BuildMessage(request);

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    _settings.Value.SmtpHost,
                    _settings.Value.SmtpPort,
                    SecureSocketOptions.StartTls,
                    cancellationToken
                );

                await smtp.AuthenticateAsync(
                    _settings.Value.SmtpUser,
                    _settings.Value.SmtpPass,
                    cancellationToken
                );

                await smtp.SendAsync(message, cancellationToken);

                await smtp.DisconnectAsync(quit: true, cancellationToken);

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Envío de email cancelado para {To}", request.To);
                throw;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al enviar email a {To} — asunto: {Subject}", request.To, request.Subject);
                throw;
            }

        }

        private MimeMessage BuildMessage(EmailRequest request)
        {
            var message = new MimeMessage();

            message.Sender = MailboxAddress.Parse(request.From ?? _settings.Value.From);
            message.From.Add(new MailboxAddress(_settings.Value.DisplayName,
                request.From ?? _settings.Value.From));
            message.To.Add(MailboxAddress.Parse(request.To));
            message.Subject = request.Subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = request.Body
            }.ToMessageBody();

            return message;
        }
    }
}
