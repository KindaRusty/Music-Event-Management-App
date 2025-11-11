using Microsoft.Extensions.Configuration;
using MusicEventManagementSystem.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
namespace MusicEventManagementSystem.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Read SendGrid settings from configuration
            _apiKey = _configuration["SendGridSettings:ApiKey"];
            _fromEmail = _configuration["SendGridSettings:FromEmail"];
            _fromName = _configuration["SendGridSettings:FromName"];
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // Check if SendGrid settings are configured
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_fromEmail))
            {
                _logger.LogError("SendGridSettings (ApiKey or FromEmail) was not configured in appsettings.json.");
                return; // Nothing to do if not configured
            }
            try
            {
                var client = new SendGridClient(_apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    HtmlContent = htmlMessage
                };
                msg.AddTo(new EmailAddress(toEmail));
                // Send the email
                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    // Write detailed error log
                    _logger.LogError("Mail sending failed. Status code: {StatusCode}, Body: {Body}",
                        response.StatusCode,
                        await response.Body.ReadAsStringAsync());
                }
                else
                {
                    _logger.LogInformation("Successfully sent to {Email}", toEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error while sending mail.");
            }
        }
    }
}