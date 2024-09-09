using Microsoft.AspNetCore.Identity.UI.Services;
using OjtPortal.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace OjtPortal.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient client;
        private readonly string _smtpServer;
        private readonly string _smtpEmail;
        private readonly string _smtpPassword;
        private readonly int _smtpPort;


        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            this._logger = logger;
            _smtpServer = _configuration["SMTP_SERVER"]!;
            _smtpEmail = _configuration["SMTP_EMAIL"]!;
            _smtpPassword = _configuration["SMTP_PASSWORD"]!;
            _smtpPort = int.Parse(_configuration["SMTP_PORT"]!);

            this.client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpEmail, _smtpPassword),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            MailMessage message = new MailMessage(_smtpEmail, email, subject, body)
            {
                IsBodyHtml = true
            };
            for (int i = 1; i <= 3; i++) {
                try
                {
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Successfully sent email on attempt {i}.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Attempt {i}: Failed to send email. {ex.Message}");
                }
            }
        }


    }
}
