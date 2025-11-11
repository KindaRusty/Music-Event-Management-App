using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Services.Implementations
{
    public class TemplateService : ITemplateService
    {
        private readonly IWebHostEnvironment _env;

        public TemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders)
        {
            // Path to the email template file
            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", $"{templateName}.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy template '{templateName}.html' tại {templatePath}");
            }

            var templateContent = new StringBuilder(await File.ReadAllTextAsync(templatePath));

            // Replace placeholders in the template
            foreach (var placeholder in placeholders)
            {
                templateContent.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return templateContent.ToString();
        }
    }
}