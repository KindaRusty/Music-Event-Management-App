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
                return new JsonResult(new { success = false, error = "Prompt không được để trống." });
            }

            try
            {
                // Dùng IAiService để tạo nội dung
                // (Bạn có thể cần tạo một phương thức mới trong IAiService cho việc này, 
                // hoặc tái sử dụng GenerateEventDescriptionAsync nếu nó phù hợp)

                // Giả sử chúng ta dùng một phương thức mới (cần thêm vào IAiService)
                // var generatedContent = await _aiService.GenerateEmailContentAsync(request.EventName, request.Prompt);

                // Tạm thời, chúng ta tái sử dụng GetChatReplyAsync
                string fullPrompt = $"Viết một email thông báo cho sự kiện '{request.EventName}' với nội dung: {request.Prompt}. Chỉ trả về nội dung email, không cần lời chào.";
                var generatedContent = await _aiService.GetChatReplyAsync(fullPrompt);

                return new JsonResult(new { success = true, content = generatedContent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi AI tạo nội dung.");
                return new JsonResult(new { success = false, error = "Lỗi dịch vụ AI." });
            }
        }

        public async Task<IActionResult> OnPostSendTemplatedEmailAsync(int? id)
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
                int successCount = 0;
                foreach (var email in recipients)
                {
                    string finalHtmlMessage;

                    if (SelectedTemplate == "EventAnnouncement")
                    {
                        // 1. Tạo dictionary placeholders
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
                        // 3. Gửi nội dung thô nếu chọn Tùy chỉnh
                        finalHtmlMessage = EmailMessage;
                    }

                    // 4. Gửi mail
                    await _emailService.SendEmailAsync(email, EmailSubject, finalHtmlMessage);
                    successCount++;
                }

                TempData["Success"] = $"Đã gửi email thành công đến {successCount} người tham dự.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email hàng loạt cho EventID {EventId}", id);
                TempData["Error"] = "Đã xảy ra lỗi khi gửi email hàng loạt.";
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