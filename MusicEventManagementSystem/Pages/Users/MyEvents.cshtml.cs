using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;

namespace MusicEventManagementSystem.Pages.Users
{
    [Authorize]
    public class MyEventsModel : PageModel
    {
        private readonly MusicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyEventsModel(MusicDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            Registrations = await _context.EventRegistrations
                .Where(r => r.UserID == userId)
                .Include(r => r.MusicEvent)
                .Include(r => r.PricingTier)
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            return Page();
        }
    }
}