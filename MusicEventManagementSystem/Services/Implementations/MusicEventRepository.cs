using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.Collections.Generic; // THÊM
using System.Linq; // THÊM
using System; // THÊM

namespace MusicEventManagementSystem.Services.Implementations
{
    public class MusicEventRepository : IMusicEventRepository
    {
        private readonly MusicDbContext _context;

        public MusicEventRepository(MusicDbContext context)
        {
            _context = context;
        }

        // ... (Giữ nguyên các phương thức Create, Delete, Update, Get... của bạn) ...

        // [CÁC PHƯƠORC CỦA BẠN ĐỂ THAM KHẢO]
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
                    throw new InvalidOperationException("Không thể xóa sự kiện đã có đăng ký.");
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
            // ... (Logic ReconcileCollection phức tạp của bạn ở đây) ...
            // (Phần này được rút gọn dựa trên file của bạn)
            var dbEvent = await _context.MusicEvents
               .Include(e => e.PricingTiers)
               .Include(e => e.RequiredFields)
               .FirstOrDefaultAsync(e => e.EventID == musicEvent.EventID);

            if (dbEvent == null) throw new KeyNotFoundException("Event not found");

            // Cập nhật scalar properties
            _context.Entry(dbEvent).CurrentValues.SetValues(musicEvent);

            // Logic Reconcile...
            // ...

            await _context.SaveChangesAsync();
        }


        // THÊM PHƯƠNG THỨC MỚI NÀY
        public async Task<IEnumerable<MusicEvent>> GetFilteredEventsAsync(string? searchTerm, string? genre)
        {
            var query = _context.MusicEvents
                                .Where(e => e.IsPublished && e.EventDate > DateTime.Now) // SỬA: Dùng EventDate
                                .AsQueryable();

            // Lọc theo từ khóa (Tên hoặc Địa điểm)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.EventName.Contains(searchTerm) || e.Location.Contains(searchTerm)); // SỬA: Dùng EventName, Location
            }

            // Lọc theo thể loại
            if (!string.IsNullOrEmpty(genre) && genre != "All")
            {
                query = query.Where(e => e.Genre == genre);
            }

            return await query.OrderBy(e => e.EventDate).ToListAsync(); // SỬA: Dùng EventDate
        }
    }
}