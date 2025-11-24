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

            var templateContent = new StringBuilder(await File.ReadAllTextAsync(templatePath));

            foreach (var placeholder in placeholders)
            {
                templateContent.Replace(placeholder.Key, placeholder.Value);
            }

            return templateContent.ToString();
        }
    }
}