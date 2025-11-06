using Microsoft.Extensions.Configuration;
using MusicEventManagementSystem.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Thêm
using System; // Thêm

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

            // Đọc cấu hình từ appsettings.json
            _apiKey = _configuration["SendGridSettings:ApiKey"];
            _fromEmail = _configuration["SendGridSettings:FromEmail"];
            _fromName = _configuration["SendGridSettings:FromName"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // Kiểm tra xem API key đã được cấu hình chưa
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_fromEmail))
            {
                _logger.LogError("SendGridSettings (ApiKey hoặc FromEmail) chưa được cấu hình trong appsettings.json.");
                return; // Không gửi nếu thiếu cấu hình
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

                // Gửi email
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    // Ghi log nếu SendGrid báo lỗi
                    _logger.LogError("Gửi email thất bại. Status code: {StatusCode}, Body: {Body}",
                        response.StatusCode,
                        await response.Body.ReadAsStringAsync());
                }
                else
                {
                    _logger.LogInformation("Đã gửi email thành công đến {Email}", toEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi gửi email.");
            }
        }
    }
}