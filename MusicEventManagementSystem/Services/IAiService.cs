using System;
using System.Collections.Generic; // THÊM
using System.Threading.Tasks;
using MusicEventManagementSystem.Models; // THÊM

namespace MusicEventManagementSystem.Services
{
    public interface IAiService
    {
        Task<string> GenerateEventDescriptionAsync(string eventTitle, string eventType);

        Task<string> GetChatReplyAsync(string userMessage);

        Task<List<MusicEvent>> GetEventRecommendationsAsync(string userId);
    }
}