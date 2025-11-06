using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // [QUAN TRỌNG] Cần cho CreateExecutionStrategy
using MusicEventManagementSystem.Data;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicEventManagementSystem.Pages.Events
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly IMusicEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MusicDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IMusicEventRepository eventRepository,
            UserManager<ApplicationUser> userManager,
            MusicDbContext context,
            ILogger<RegisterModel> logger)
        {
            _eventRepository = eventRepository;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public MusicEvent MusicEvent { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public int EventID { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn một loại vé")]
            public int SelectedPricingTierID { get; set; }

            public Dictionary<int, string> DynamicData { get; set; } = new Dictionary<int, string>();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var musicEvent = await _eventRepository.GetEventWithDetailsAsync(id);

            if (musicEvent == null || !musicEvent.IsPublished)
            {
                return NotFound("Không tìm thấy sự kiện hoặc sự kiện chưa được công bố.");
            }

            MusicEvent = musicEvent;
            Input.EventID = musicEvent.EventID;

            foreach (var field in MusicEvent.RequiredFields)
            {
                Input.DynamicData[field.FieldID] = string.Empty;
            }

            return Page();
        }

        // [VIẾT LẠI HOÀN TOÀN ĐỂ FIX LỖI EXECUTION STRATEGY]
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Tải lại thông tin sự kiện đầy đủ để xác thực
            var musicEvent = await _eventRepository.GetEventWithDetailsAsync(Input.EventID);
            if (musicEvent == null)
            {
                return NotFound("Sự kiện không tồn tại.");
            }
            MusicEvent = musicEvent; // Gán lại để render nếu có lỗi

            // Xác thực loại vé
            var selectedTier = musicEvent.PricingTiers
                .FirstOrDefault(p => p.PricingTierID == Input.SelectedPricingTierID);

            if (selectedTier == null)
            {
                ModelState.AddModelError(string.Empty, "Loại vé bạn chọn không hợp lệ.");
            }

            // Xác thực các trường động (Dynamic Fields)
            foreach (var requiredField in musicEvent.RequiredFields)
            {
                if (requiredField.IsRequired)
                {
                    Input.DynamicData.TryGetValue(requiredField.FieldID, out var fieldValue);
                    if (requiredField.FieldType == "Checkbox" && fieldValue != "true")
                    {
                        ModelState.AddModelError($"Input.DynamicData[{requiredField.FieldID}]", $"Bạn phải đồng ý với '{requiredField.FieldName}'.");
                    }
                    else if (requiredField.FieldType != "Checkbox" && string.IsNullOrWhiteSpace(fieldValue))
                    {
                        ModelState.AddModelError($"Input.DynamicData[{requiredField.FieldID}]", $"{requiredField.FieldName} là bắt buộc.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return Page(); // Render lại trang với các lỗi
            }

            // [SỬA LỖI] Lấy chiến lược (strategy) từ DbContext
            var strategy = _context.Database.CreateExecutionStrategy();

            // Thực thi toàn bộ logic trong một khối "ExecuteAsync"
            // Strategy này sẽ tự động quản lý transaction và retry
            var result = await strategy.ExecuteAsync(async () =>
            {
                // Bắt đầu transaction BÊN TRONG strategy
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Tải lại loại vé BÊN TRONG transaction để khóa dòng (lock)
                    var tierToUpdate = await _context.PricingTiers
                        .FirstOrDefaultAsync(p => p.PricingTierID == Input.SelectedPricingTierID);

                    if (tierToUpdate == null)
                    {
                        ModelState.AddModelError(string.Empty, "Loại vé không còn hợp lệ.");
                        await transaction.RollbackAsync();
                        return (IActionResult)Page();
                    }

                    // Kiểm tra vé
                    tierToUpdate.SoldTickets++;
                    if (tierToUpdate.AvailableTickets.HasValue && tierToUpdate.SoldTickets > tierToUpdate.AvailableTickets)
                    {
                        ModelState.AddModelError(string.Empty, "Rất tiếc, loại vé này đã hết trong lúc bạn đăng ký.");
                        await transaction.RollbackAsync();
                        return (IActionResult)Page();
                    }

                    // Tạo đối tượng EventRegistration
                    var registration = new EventRegistration
                    {
                        EventID = musicEvent.EventID,
                        UserID = user.Id,
                        PricingTierID = tierToUpdate.PricingTierID,
                        RegistrationDate = DateTime.UtcNow,
                        TotalPrice = tierToUpdate.Price,
                        Status = "Pending",
                        PaymentStatus = "Unpaid"
                    };

                    // Tạo danh sách RegistrationData
                    foreach (var data in Input.DynamicData)
                    {
                        if (!string.IsNullOrWhiteSpace(data.Value))
                        {
                            registration.RegistrationData.Add(new RegistrationData
                            {
                                FieldID = data.Key,
                                FieldValue = data.Value
                            });
                        }
                    }

                    // Lưu Registration (và RegistrationData, và cập nhật PricingTier)
                    _context.EventRegistrations.Add(registration);

                    // Lưu tất cả thay đổi vào DB
                    await _context.SaveChangesAsync();

                    // Hoàn tất Transaction
                    await transaction.CommitAsync();

                    _logger.LogInformation("User {UserId} đã đăng ký thành công sự kiện {EventId}", user.Id, musicEvent.EventID);

                    // Chuyển hướng đến trang thành công
                    return (IActionResult)RedirectToPage("/Events/RegistrationSuccess", new { id = registration.RegistrationID });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Lỗi nghiêm trọng khi user {UserId} đăng ký sự kiện {EventId}", user.Id, Input.EventID);
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                    return (IActionResult)Page();
                }
            });

            // Trả về kết quả từ bên trong strategy
            return result!;
        }
    }
}