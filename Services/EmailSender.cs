using Microsoft.AspNetCore.Identity.UI.Services;

namespace Wheels_in_Csharp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Email seria enviado para: {email}");
            _logger.LogInformation($"Assunto: {subject}");
            _logger.LogInformation($"Mensagem: {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}