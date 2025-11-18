using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;

namespace MusicEventManagementSystem.Pages.Events
{
    public class ConfirmPaymentModel : PageModel
    {
        private readonly MusicDbContext _context;

        public ConfirmPaymentModel(MusicDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string? code)
        {
            if (string.IsNullOrEmpty(code))
            {
                Message = "Invalid or missing confirmation link.";
                return Page();
            }

            var registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)
                .FirstOrDefaultAsync(r => r.ConfirmationCode == code);

            if (registration == null)
            {
                Message = "Your registration could not be found. The link may be expired or incorrect.";
                return Page();
            }

            if (registration.PaymentStatus == "Confirmed")
            {
                Message = $"The registration for '{registration.MusicEvent.EventName}' has already been confirmed. Thank you!";
                return Page();
            }

            registration.PaymentStatus = "Confirmed";

            await _context.SaveChangesAsync();
            Message = $"Payment for '{registration.MusicEvent.EventName}' confirmed successfully! Thank you for registering.";
            return Page();
        }
    }
}