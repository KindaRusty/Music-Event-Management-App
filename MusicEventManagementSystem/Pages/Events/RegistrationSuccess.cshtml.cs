using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegistrationSuccessModel : PageModel
    {
        private readonly MusicDbContext _context;

        public RegistrationSuccessModel(MusicDbContext context)
        {
            _context = context;
        }

        // Used to display information
        public EventRegistration Registration { get; set; } = default!;
        public List<RegistrationData> DynamicData { get; set; } = new List<RegistrationData>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound("Registration ID not found.");
            }

            // Load registration info ALONG WITH related info
            Registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)        // Get event name
                .Include(r => r.ApplicationUser)  // Get user email
                .Include(r => r.PricingTier)      // Get ticket tier name
                .FirstOrDefaultAsync(r => r.RegistrationID == id.Value);

            if (Registration == null)
            {
                // Maybe the user is trying to view someone else's registration
                return Forbid("You do not have permission to view this registration or it does not exist.");
            }

            // Load dynamic fields separately
            // (Including the field name/label)
            DynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField) // Get the field name (Label)
                .Where(d => d.RegistrationID == id.Value)
                .OrderBy(d => d.RequiredField.DisplayOrder)
                .ToListAsync();

            return Page();
        }
    }
}