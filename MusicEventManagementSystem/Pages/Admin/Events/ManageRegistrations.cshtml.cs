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
        private readonly ITemplateService _templateService;
        private readonly IAiService _aiService;

        public ManageRegistrationsModel(
            MusicDbContext context,
            IEmailService emailService,
            ILogger<ManageRegistrationsModel> logger,
            ITemplateService templateService,
            IAiService aiService)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _templateService = templateService;
            _aiService = aiService;
        }

        public MusicEvent MusicEvent { get; set; }
        public List<EventRegistration> Registrations { get; set; }

        [BindProperty]
        public string EmailSubject { get; set; }
        [BindProperty]
        public string EmailMessage { get; set; }

        [BindProperty]
        public string SelectedTemplate { get; set; } = "Custom";

        [BindProperty]
        public string AiPrompt { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            MusicEvent = await _context.MusicEvents.FirstOrDefaultAsync(e => e.EventID == id);
            if (MusicEvent == null) return NotFound();

            Registrations = await _context.EventRegistrations
                .Where(r => r.EventID == id)
                .Include(r => r.ApplicationUser)
                .Include(r => r.PricingTier)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostGenerateAiContentAsync([FromBody] AiPromptRequest request)
        {
            if (string.IsNullOrEmpty(request.Prompt))
            {
                return new JsonResult(new { success = false, error = "Prompt cannot be empty." });
            }

            try
            {
                // Use IAiService to generate content
                // (You might need to create a new method in IAiService for this,
                // or reuse GenerateEventDescriptionAsync if it fits)

                // Assuming we use a new method (needs adding to IAiService)
                // var generatedContent = await _aiService.GenerateEmailContentAsync(request.EventName, request.Prompt);

                // Temporarily, we reuse GetChatReplyAsync
                string fullPrompt = $"Write an email notification for the event '{request.EventName}' with the content: {request.Prompt}. Only return the email content, without any greeting.";
                var generatedContent = await _aiService.GetChatReplyAsync(fullPrompt);

                return new JsonResult(new { success = true, content = generatedContent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI to generate content.");
                return new JsonResult(new { success = false, error = "AI service error." });
            }
        }

        public async Task<IActionResult> OnPostSendTemplatedEmailAsync(int? id)
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
                int successCount = 0;
                foreach (var email in recipients)
                {
                    string finalHtmlMessage;

                    if (SelectedTemplate == "EventAnnouncement")
                    {
                        // 1. Create placeholders dictionary
                        var placeholders = new Dictionary<string, string>
                        {
                            { "EventName", MusicEvent.EventName },
                            { "CustomMessage", EmailMessage },
                            { "EventDate", MusicEvent.EventDate.ToString("g") },
                            { "EventLocation", MusicEvent.Location }
                        };

                        // 2. Load template
                        finalHtmlMessage = await _templateService.LoadTemplateAsync("EventAnnouncement", placeholders);
                    }
                    else // (SelectedTemplate == "Custom")
                    {
                        // 3. Send raw content if Custom is selected
                        finalHtmlMessage = EmailMessage;
                    }

                    // 4. Send mail
                    await _emailService.SendEmailAsync(email, EmailSubject, finalHtmlMessage);
                    successCount++;
                }

                TempData["Success"] = $"Successfully sent email to {successCount} attendees.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk email for EventID {EventId}", id);
                TempData["Error"] = "An error occurred while sending the bulk email.";
            }

            return RedirectToPage(new { id = id });
        }
    }
    public class AiPromptRequest
    {
        public string Prompt { get; set; }
        public string EventName { get; set; }
    }
}