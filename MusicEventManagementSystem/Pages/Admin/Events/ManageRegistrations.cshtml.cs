using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class ManageRegistrationsModel : PageModel
    {
        private readonly MusicDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ManageRegistrationsModel> _logger;
        public ManageRegistrationsModel(MusicDbContext context,
                                        IEmailService emailService,
                                        ILogger<ManageRegistrationsModel> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public MusicEvent MusicEvent { get; set; }
        public List<EventRegistration> Registrations { get; set; }

        [BindProperty]
        public string EmailSubject { get; set; }
        [BindProperty]
        public string EmailMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MusicEvent = await _context.MusicEvents.FirstOrDefaultAsync(e => e.EventID == id);

            if (MusicEvent == null)
            {
                return NotFound();
            }

            Registrations = await _context.EventRegistrations
                .Where(r => r.EventID == id)
                .Include(r => r.ApplicationUser)
                .Include(r => r.PricingTier)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSendEmailAsync(int? id)
        {
            if (id == null) return NotFound();

            if (string.IsNullOrEmpty(EmailSubject) || string.IsNullOrEmpty(EmailMessage))
            {
                TempData["Error"] = "Subject and Email Message cannot be empty.";
                return RedirectToPage(new { id = id });
            }

            await OnGetAsync(id);

            if (Registrations == null || !Registrations.Any())
            {
                TempData["Error"] = "There are no attendees registered for this event to send an email to.";
                return RedirectToPage(new { id = id });
            }

            var recipients = Registrations
                .Where(r => r.ApplicationUser != null && !string.IsNullOrEmpty(r.ApplicationUser.Email))
                .Select(r => r.ApplicationUser.Email)
                .Distinct()
                .ToList();

            if (!recipients.Any())
            {
                TempData["Error"] = "No valid attendee emails found.";
                return RedirectToPage(new { id = id });
            }

            try
            {
                foreach (var email in recipients)
                {
                    await _emailService.SendEmailAsync(email, EmailSubject, EmailMessage);
                }

                TempData["Success"] = $"Successfully sent email to {recipients.Count} attendees.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk email for EventID {EventId}", id);
                TempData["Error"] = "An error occurred while sending the bulk email.";
            }

            return RedirectToPage(new { id = id });
        }
    }
}