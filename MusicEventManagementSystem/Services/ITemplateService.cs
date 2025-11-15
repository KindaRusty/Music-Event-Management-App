namespace MusicEventManagementSystem.Services
{
    public interface ITemplateService
    {
        Task<string> LoadTemplateAsync(string templateName, Dictionary<string, string> placeholders);
    }
}