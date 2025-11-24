using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegistrationSuccessModel : PageModel
    {
        private readonly MusicDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;
        public RegistrationSuccessModel(MusicDbContext context, IEmailService emailService, ITemplateService templateService)
        {
            _context = context;
            _emailService = emailService;
            _templateService = templateService;
        }

        public EventRegistration Registration { get; set; } = default!;
        public List<RegistrationData> DynamicData { get; set; } = new List<RegistrationData>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound("Registration ID not found.");
            }

            Registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)
                .Include(r => r.ApplicationUser)
                .Include(r => r.PricingTier)
                .FirstOrDefaultAsync(r => r.RegistrationID == id.Value);

            if (Registration == null)
            {
                return Forbid("You do not have permission to view this registration or it does not exist.");
            }
            DynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField)
                .Where(d => d.RegistrationID == id.Value)
                .OrderBy(d => d.RequiredField.DisplayOrder)
                .ToListAsync();
            string recipientEmail = string.Empty;
            var emailField = DynamicData.FirstOrDefault(d =>
                d.RequiredField.FieldName.Equals("Email", StringComparison.OrdinalIgnoreCase));

            if (emailField != null && !string.IsNullOrEmpty(emailField.FieldValue))
            {
                recipientEmail = emailField.FieldValue;
            }
            else
            {
                recipientEmail = Registration.ApplicationUser?.Email;
            }
            if (Registration.PaymentStatus != "Confirmed" && !string.IsNullOrEmpty(recipientEmail))
            {
                // 1. Generate confirmation link
                var confirmationLink = Url.Page(
                    "/Events/ConfirmPayment",
                    null,
                    new { code = Registration.ConfirmationCode },
                    Request.Scheme);

                // 2. Create placeholders
                var placeholders = new Dictionary<string, string>
                {
                    { "{{EventName}}", Registration.MusicEvent.EventName },
                    { "{{TierName}}", Registration.PricingTier.TierName },
                    { "{{TotalPrice}}", Registration.TotalPrice.ToString("N0") },
                    { "{{RegistrationID}}", Registration.RegistrationID.ToString() },
                    { "{{ConfirmationLink}}", confirmationLink ?? "#" }
                };

                // 3. Load template
                string emailBody = await _templateService.LoadTemplateAsync("RegistrationPayment", placeholders);

                // 4. Send email
                await _emailService.SendEmailAsync(
                    recipientEmail,
                    $"Payment Confirmation for: {Registration.MusicEvent.EventName}",
                    emailBody);
            }
            return Page();
        }
    }
}