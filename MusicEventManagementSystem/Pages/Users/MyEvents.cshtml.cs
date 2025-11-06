using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.User
{
    [Authorize]
    public class MyEventsModel : PageModel
    {
        private readonly MusicDbContext _context;

        public MyEventsModel(MusicDbContext context)
        {
            _context = context;
        }

        public List<EventRegistration> MyRegistrations { get; set; } = new List<EventRegistration>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            MyRegistrations = await _context.EventRegistrations
                .Where(r => r.UserID == userId) // SỬA: Dùng UserID
                .Include(r => r.MusicEvent)
                .Include(r => r.PricingTier)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            return Page();
        }
    }
}