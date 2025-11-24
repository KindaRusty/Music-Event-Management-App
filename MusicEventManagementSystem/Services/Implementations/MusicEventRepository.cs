using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;

namespace MusicEventManagementSystem.Services.Implementations
{
    public class MusicEventRepository : IMusicEventRepository
    {
        private readonly MusicDbContext _context;
        public MusicEventRepository(MusicDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateEventAsync(MusicEvent musicEvent)
        {
            _context.MusicEvents.Add(musicEvent);
            await _context.SaveChangesAsync();
            return musicEvent.EventID;
        }
        public async Task DeleteEventAsync(int id)
        {
            var musicEvent = await _context.MusicEvents.FindAsync(id);
            if (musicEvent != null)
            {
                var hasRegistrations = await _context.EventRegistrations.AnyAsync(r => r.EventID == id);
                if (hasRegistrations)
                {
                    throw new InvalidOperationException("Cannot delete an event that has existing registrations.");
                }
                _context.MusicEvents.Remove(musicEvent);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<MusicEvent>> GetAllEventsForAdminAsync()
        {
            return await _context.MusicEvents
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }
        public async Task<MusicEvent?> GetEventByIdAsync(int id)
        {
            return await _context.MusicEvents.FindAsync(id);
        }
        public async Task<MusicEvent?> GetEventWithDetailsAsync(int id)
        {
            return await _context.MusicEvents
                .Include(e => e.PricingTiers)
                .Include(e => e.RequiredFields)
                .FirstOrDefaultAsync(e => e.EventID == id);
        }
        public async Task<IEnumerable<MusicEvent>> GetPublishedEventsAsync()
        {
            return await _context.MusicEvents
                .Where(e => e.IsPublished && e.EventDate > DateTime.Now)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }
        public async Task UpdateEventAsync(MusicEvent musicEvent)
        {
            var dbEvent = await _context.MusicEvents
               .Include(e => e.PricingTiers)
               .Include(e => e.RequiredFields)
               .FirstOrDefaultAsync(e => e.EventID == musicEvent.EventID);

            if (dbEvent == null) throw new KeyNotFoundException("Event not found");

            _context.Entry(dbEvent).CurrentValues.SetValues(musicEvent);

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<MusicEvent>> GetFilteredEventsAsync(string? searchTerm, string? genre)
        {
            var query = _context.MusicEvents
                                .Where(e => e.IsPublished && e.EventDate > DateTime.Now)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.EventName.Contains(searchTerm) || e.Location.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(e => e.Genre == genre);
            }
            return await query.OrderBy(e => e.EventDate).ToListAsync();
        }
    }
}