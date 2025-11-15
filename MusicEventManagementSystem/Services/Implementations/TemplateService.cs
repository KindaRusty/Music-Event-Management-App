using System.Text;

namespace MusicEventManagementSystem.Services.Implementations
{
    public class TemplateService : ITemplateService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TemplateService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders)
        {
            var templatePath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "EmailTemplates",
                $"{templateName}.html");

            if (!File.Exists(templatePath))
            {
                return $"Template {templateName} not found.";
            }

            // Dùng StringBuilder để tối ưu việc thay thế
            var templateContent = new StringBuilder(await File.ReadAllTextAsync(templatePath));

            // --- SỬA LỖI: Thêm vòng lặp để thay thế placeholder ---
            // Đây là lý do email của bạn bị trống
            foreach (var placeholder in placeholders)
            {
                templateContent.Replace(placeholder.Key, placeholder.Value);
            }
            // --- Kết thúc sửa lỗi ---

            return templateContent.ToString();
        }
    }
}