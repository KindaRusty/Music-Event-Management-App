using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services;
using System; // <-- Thêm
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegistrationSuccessModel : PageModel
    {
        private readonly MusicDbContext _context;
        // --- Thêm Service ---
        private readonly IEmailService _emailService;
        private readonly ITemplateService _templateService;

        // --- Cập nhật Constructor ---
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

            // --- Tải DynamicData LÊN TRƯỚC ---
            DynamicData = await _context.RegistrationData
                .Include(d => d.RequiredField)
                .Where(d => d.RegistrationID == id.Value)
                .OrderBy(d => d.RequiredField.DisplayOrder)
                .ToListAsync();

            // --- YÊU CẦU MỚI: Tìm email xác nhận ---
            string recipientEmail = string.Empty;

            // Giả sử trường email bạn thu thập có FieldName là "Email"
            var emailField = DynamicData.FirstOrDefault(d =>
                d.RequiredField.FieldName.Equals("Email", StringComparison.OrdinalIgnoreCase));

            if (emailField != null && !string.IsNullOrEmpty(emailField.FieldValue))
            {
                recipientEmail = emailField.FieldValue; // Ưu tiên email trong form
            }
            else
            {
                // Nếu không có, dùng email của tài khoản làm dự phòng
                recipientEmail = Registration.ApplicationUser?.Email;
            }


            // --- GỬI EMAIL (Logic đã sửa) ---
            if (Registration.PaymentStatus != "Confirmed" && !string.IsNullOrEmpty(recipientEmail))
            {
                // 1. Tạo link
                // <<< ĐÂY LÀ SỬA LỖI CHO BẠN: Phải chỉ định rõ trang "ConfirmPayment"
                var confirmationLink = Url.Page(
                    "/Events/ConfirmPayment", // <-- Sửa lỗi link: Trỏ đến đúng trang
                    null,
                    new { code = Registration.ConfirmationCode },
                    Request.Scheme);

                // 2. Tạo placeholder
                var placeholders = new Dictionary<string, string>
                {
                    { "{{EventName}}", Registration.MusicEvent.EventName },
                    { "{{TierName}}", Registration.PricingTier.TierName },
                    { "{{TotalPrice}}", Registration.TotalPrice.ToString("N0") },
                    { "{{RegistrationID}}", Registration.RegistrationID.ToString() },
                    { "{{ConfirmationLink}}", confirmationLink ?? "#" }
                };

                // 3. Tải template
                string emailBody = await _templateService.LoadTemplateAsync("RegistrationPayment", placeholders);

                // 4. Gửi email
                await _emailService.SendEmailAsync(
                    recipientEmail, // <-- Gửi đến email đã tìm thấy
                    $"Payment Confirmation for: {Registration.MusicEvent.EventName}",
                    emailBody);
            }
            // --- KẾT THÚC GỬI EMAIL ---

            return Page();
        }
    }
}