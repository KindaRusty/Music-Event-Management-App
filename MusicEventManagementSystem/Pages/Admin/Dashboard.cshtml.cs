using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MusicEventManagementSystem.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicEventManagementSystem.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly MusicDbContext _context;

        public DashboardModel(MusicDbContext context)
        {
            _context = context;
        }

        // --- Các thuộc tính cho bộ lọc ---
        [BindProperty(SupportsGet = true)]
        public int SelectedEventId { get; set; } = 0; // 0 = "All Events"

        public SelectList EventOptions { get; set; }

        // --- Các số liệu thống kê tổng quan (sẽ được lọc) ---
        public decimal TotalRevenue { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }

        // --- Dữ liệu cho các biểu đồ ---
        public string EventRegistrationChartData { get; set; } // Biểu đồ cột 1
        public string RevenuePerEventChartData { get; set; } // Biểu đồ cột 2
        public string DailyRegistrationChartData { get; set; } // Biểu đồ đường (Line chart) MỚI

        public async Task OnGetAsync()
        {
            // --- 1. Tải danh sách sự kiện cho bộ lọc (Dropdown) ---
            var events = await _context.MusicEvents
                                   .OrderBy(e => e.EventName)
                                   .Select(e => new { e.EventID, e.EventName })
                                   .ToListAsync();

            var eventListItems = events.Select(e => new SelectListItem
            {
                Value = e.EventID.ToString(),
                Text = e.EventName
            }).ToList();

            eventListItems.Insert(0, new SelectListItem { Value = "0", Text = "--- Tất cả sự kiện ---" });
            EventOptions = new SelectList(eventListItems, "Value", "Text", SelectedEventId);

            // --- 2. Xây dựng các truy vấn cơ sở đã được lọc ---

            IQueryable<EventRegistration> baseRegQuery = _context.EventRegistrations
                                                                 .Include(r => r.PricingTier);

            IQueryable<MusicEvent> baseEventQuery = _context.MusicEvents;

            if (SelectedEventId > 0)
            {
                baseRegQuery = baseRegQuery.Where(r => r.EventID == SelectedEventId);
                baseEventQuery = baseEventQuery.Where(e => e.EventID == SelectedEventId);
            }

            // --- 3. Tính toán các số liệu thống kê tổng quan (dựa trên truy vấn đã lọc) ---
            TotalRegistrations = await baseRegQuery.CountAsync();
            TotalEvents = await baseEventQuery.CountAsync();
            TotalUsers = await _context.Users.CountAsync();

            if (await baseRegQuery.AnyAsync())
            {
                // Giả định TotalPrice trên EventRegistration đã được tính toán khi đăng ký
                // Hoặc nếu không, chúng ta quay lại tính qua PricingTier
                // TotalRevenue = await baseRegQuery.SumAsync(r => r.TotalPrice); 
                // ---- HOẶC ----
                TotalRevenue = await baseRegQuery.SumAsync(r => r.PricingTier.Price);
            }
            else
            {
                TotalRevenue = 0;
            }

            // --- 4. Chuẩn bị dữ liệu cho Biểu đồ Cột (chỉ hiển thị khi chọn "All Events") ---
            if (SelectedEventId == 0)
            {
                var eventRegistrations = await baseEventQuery
                    .Include(e => e.Registrations)
                    .Select(e => new
                    {
                        EventName = e.EventName, // Đã sửa
                        RegistrationCount = e.Registrations.Count()
                    })
                    .Where(e => e.RegistrationCount > 0)
                    .OrderByDescending(e => e.RegistrationCount)
                    .ToListAsync();
                EventRegistrationChartData = JsonSerializer.Serialize(eventRegistrations);

                var eventRevenue = await baseEventQuery
                    .Include(e => e.Registrations)
                        .ThenInclude(r => r.PricingTier)
                    .Select(e => new
                    {
                        EventName = e.EventName, // Đã sửa
                        EventRevenue = e.Registrations.Sum(r => r.PricingTier.Price)
                    })
                    .Where(e => e.EventRevenue > 0)
                    .OrderByDescending(e => e.EventRevenue)
                    .ToListAsync();
                RevenuePerEventChartData = JsonSerializer.Serialize(eventRevenue);
            }
            else
            {
                EventRegistrationChartData = "[]";
                RevenuePerEventChartData = "[]";
            }

            // --- 5. (MỚI) Chuẩn bị dữ liệu cho Biểu đồ đường: Đăng ký theo ngày ---
            var dailyData = await baseRegQuery
                // ***** ĐÂY LÀ CHỖ SỬA LỖI (LẦN NỮA) *****
                .GroupBy(r => r.RegistrationDate.Date) // Sửa trở lại thành RegistrationDate
                .Select(g => new
                {
                    Date = g.Key,
                    RegistrationCount = g.Count(),
                    DailyRevenue = g.Sum(r => r.PricingTier.Price),
                    AverageTicketPrice = g.Average(r => r.PricingTier.Price)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            DailyRegistrationChartData = JsonSerializer.Serialize(dailyData);
        }
    }
}