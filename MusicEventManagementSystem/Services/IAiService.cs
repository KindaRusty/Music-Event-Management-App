using MusicEventManagementSystem.Models;
namespace MusicEventManagementSystem.Services
{
    public interface IAiService
    {
        Task<string> GenerateEventDescriptionAsync(string eventTitle, string eventType);
        Task<string> GetChatReplyAsync(string userMessage);
        Task<List<MusicEvent>> GetEventRecommendationsAsync(string userId);
    }
}