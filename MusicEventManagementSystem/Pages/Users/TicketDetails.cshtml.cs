using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // <-- Thêm cái này để dùng Guid

namespace MusicEventManagementSystem.Pages.Users
{
    [Authorize]
    public class TicketDetailsModel : PageModel
    {
        private readonly MusicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        // Phải có 2 service này
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;

        // Constructor phải nhận CẢ 4 service
        public TicketDetailsModel(
            MusicDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ITemplateService templateService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _templateService = templateService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public EventRegistration Registration { get; set; } = default!;
        public List<RegistrationData> DynamicData { get; set; } = new List<RegistrationData>();

        // (Hàm OnGetAsync của bạn đã đúng, giữ nguyên)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound("Registration ID not found.");
            }
            var userId = _userManager.GetUserId(User);
            Registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)
                .Include(r => r.ApplicationUser)
                .Include(r => r.PricingTier)
                .FirstOrDefaultAsync(r => r.RegistrationID == id.Value);

            if (Registration == null || Registration.UserID != userId)
            {
                return Forbid("You do not have permission to view this ticket or it does not exist.");
            }
            DynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField)
                .Where(d => d.RegistrationID == id.Value)
                .OrderBy(d => d.RequiredField.DisplayOrder)
                .ToListAsync();

            return Page();
        }

        // --- HÀM ONPOST ĐÃ SỬA LỖI DỮ LIỆU CŨ ---
        public async Task<IActionResult> OnPostResendEmailAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _context.EventRegistrations
                .Include(r => r.MusicEvent)
                .Include(r => r.ApplicationUser)
                .Include(r => r.PricingTier)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);

            if (registration == null || registration.UserID != userId)
            {
                return Forbid();
            }

            if (registration.PaymentStatus == "Confirmed")
            {
                StatusMessage = "Error: This ticket is already paid.";
                return RedirectToPage(new { id = id });
            }

            // --- SỬA LỖI QUAN TRỌNG ---
            // Kiểm tra xem vé cũ này có mã xác nhận không.
            // Nếu không (vì nó được tạo trước khi bạn sửa code Register),
            // hãy tạo một mã MỚI và LƯU nó vào database.
            if (string.IsNullOrEmpty(registration.ConfirmationCode))
            {
                registration.ConfirmationCode = Guid.NewGuid().ToString();
                _context.EventRegistrations.Update(registration);
                await _context.SaveChangesAsync();
                // Bây giờ, vé này đã được "sửa chữa" trong database.
            }
            // --- KẾT THÚC SỬA LỖI ---

            // Tìm email (logic này đã đúng)
            var dynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField)
                .Where(d => d.RegistrationID == id)
                .ToListAsync();
            var emailField = dynamicData.FirstOrDefault(d =>
                d.RequiredField.FieldName.Equals("Email", System.StringComparison.OrdinalIgnoreCase));
            string recipientEmail = emailField?.FieldValue;
            if (string.IsNullOrEmpty(recipientEmail))
            {
                recipientEmail = registration.ApplicationUser?.Email;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                StatusMessage = "Error: Could not find a recipient email address.";
                return RedirectToPage(new { id = id });
            }

            // Dòng này bây giờ sẽ an toàn vì ConfirmationCode không bao giờ null
            var confirmationLink = Url.Page(
                "/Events/ConfirmPayment",
                null,
                new { code = registration.ConfirmationCode },
                Request.Scheme);

            var placeholders = new Dictionary<string, string>
            {
                { "{{EventName}}", registration.MusicEvent.EventName },
                { "{{TierName}}", registration.PricingTier.TierName },
                { "{{TotalPrice}}", registration.TotalPrice.ToString("N0") },
                { "{{RegistrationID}}", registration.RegistrationID.ToString() },
                { "{{ConfirmationLink}}", confirmationLink ?? "#" }
            };

            string emailBody = await _templateService.LoadTemplateAsync("RegistrationPayment", placeholders);
            await _emailService.SendEmailAsync(
                recipientEmail,
                $"Payment Confirmation for: {registration.MusicEvent.EventName}",
                emailBody);

            StatusMessage = "Confirmation email has been resent successfully.";
            return RedirectToPage(new { id = id });
        }
    }
}