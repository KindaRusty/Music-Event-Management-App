using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MusicEventManagementSystem.Models;
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
        [BindProperty(SupportsGet = true)]
        public int SelectedEventId { get; set; } = 0;

        public SelectList EventOptions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public string EventRegistrationChartData { get; set; }
        public string RevenuePerEventChartData { get; set; }
        public string DailyRegistrationChartData { get; set; }

        public async Task OnGetAsync()
        {
            var events = await _context.MusicEvents
                                   .OrderBy(e => e.EventName)
                                   .Select(e => new { e.EventID, e.EventName })
                                   .ToListAsync();

            var eventListItems = events.Select(e => new SelectListItem
            {
                Value = e.EventID.ToString(),
                Text = e.EventName
            }).ToList();

            eventListItems.Insert(0, new SelectListItem { Value = "0", Text = "--- All Events ---" });
            EventOptions = new SelectList(eventListItems, "Value", "Text", SelectedEventId);

            IQueryable<EventRegistration> baseRegQuery = _context.EventRegistrations
                                                                 .Include(r => r.PricingTier);

            IQueryable<MusicEvent> baseEventQuery = _context.MusicEvents;

            if (SelectedEventId > 0)
            {
                baseRegQuery = baseRegQuery.Where(r => r.EventID == SelectedEventId);
                baseEventQuery = baseEventQuery.Where(e => e.EventID == SelectedEventId);
            }
            TotalRegistrations = await baseRegQuery.CountAsync();
            TotalEvents = await baseEventQuery.CountAsync();
            TotalUsers = await _context.Users.CountAsync();

            if (await baseRegQuery.AnyAsync())
            {
                TotalRevenue = await baseRegQuery.SumAsync(r => r.PricingTier.Price);
            }
            else
            {
                TotalRevenue = 0;
            }
            if (SelectedEventId == 0)
            {
                var eventRegistrations = await baseEventQuery
                    .Include(e => e.Registrations)
                    .Select(e => new
                    {
                        EventName = e.EventName,
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
                        EventName = e.EventName,
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
            var dailyData = await baseRegQuery
                .GroupBy(r => r.RegistrationDate.Date)
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