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
            // Para desenvolvimento, apenas loga o email que seria enviado
            _logger.LogInformation($"Email seria enviado para: {email}");
            _logger.LogInformation($"Assunto: {subject}");
            _logger.LogInformation($"Mensagem: {htmlMessage}");

            // Simula envio bem-sucedido
            return Task.CompletedTask;
        }
    }
}