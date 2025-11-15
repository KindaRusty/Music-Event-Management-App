using MusicEventManagementSystem.Models;
namespace MusicEventManagementSystem.Services.Interfaces
{
    public interface IMusicEventRepository
    {
        Task<IEnumerable<MusicEvent>> GetPublishedEventsAsync();
        Task<MusicEvent?> GetEventByIdAsync(int id);
        Task<int> CreateEventAsync(MusicEvent musicEvent);
        Task UpdateEventAsync(MusicEvent musicEvent);
        Task DeleteEventAsync(int id);
        Task<MusicEvent?> GetEventWithDetailsAsync(int id);
        Task<IEnumerable<MusicEvent>> GetAllEventsForAdminAsync();
        Task<IEnumerable<MusicEvent>> GetFilteredEventsAsync(string? searchTerm, string? genre);
    }
}