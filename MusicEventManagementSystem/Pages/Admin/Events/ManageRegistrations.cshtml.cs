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
using Microsoft.Extensions.Logging; // THÊM DÒNG NÀY

namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class ManageRegistrationsModel : PageModel
    {
        private readonly MusicDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ManageRegistrationsModel> _logger; // THÊM DÒNG NÀY

        // SỬA LẠI CONSTRUCTOR
        public ManageRegistrationsModel(MusicDbContext context,
                                        IEmailService emailService,
                                        ILogger<ManageRegistrationsModel> logger) // THÊM LOGGER
        {
            _context = context;
            _emailService = emailService;
            _logger = logger; // THÊM DÒNG NÀY
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
                TempData["Error"] = "Chủ đề và Nội dung email không được để trống.";
                return RedirectToPage(new { id = id });
            }

            await OnGetAsync(id);

            if (Registrations == null || !Registrations.Any())
            {
                TempData["Error"] = "Không có ai đăng ký sự kiện này để gửi email.";
                return RedirectToPage(new { id = id });
            }

            var recipients = Registrations
                .Where(r => r.ApplicationUser != null && !string.IsNullOrEmpty(r.ApplicationUser.Email))
                .Select(r => r.ApplicationUser.Email)
                .Distinct()
                .ToList();

            if (!recipients.Any())
            {
                TempData["Error"] = "Không tìm thấy email hợp lệ nào của người tham dự.";
                return RedirectToPage(new { id = id });
            }

            try
            {
                foreach (var email in recipients)
                {
                    await _emailService.SendEmailAsync(email, EmailSubject, EmailMessage);
                }

                TempData["Success"] = $"Đã gửi email thành công đến {recipients.Count} người tham dự.";
            }
            catch (Exception ex)
            {
                // SỬA LỖI: Bây giờ _logger đã tồn tại
                _logger.LogError(ex, "Lỗi khi gửi email hàng loạt cho EventID {EventId}", id);
                TempData["Error"] = "Đã xảy ra lỗi khi gửi email hàng loạt.";
            }

            return RedirectToPage(new { id = id });
        }
    }
}