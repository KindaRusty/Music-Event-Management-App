using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using MusicEventManagementSystem.Services;


namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IMusicEventRepository _repository;
        private readonly IAiService _aiService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IMusicEventRepository repository,
            IAiService aiService,
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _repository = repository;
            _aiService = aiService;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // [MỚI] Bind danh sách các loại vé
        [BindProperty]
        public List<PricingTierInput> PricingTiers { get; set; } = new List<PricingTierInput>();

        // [MỚI] Bind danh sách các trường bắt buộc
        [BindProperty]
        public List<RequiredFieldInput> RequiredFields { get; set; } = new List<RequiredFieldInput>();

        #region Input DTOs
        public class InputModel
        {
            [Required(ErrorMessage = "Tên sự kiện là bắt buộc")]
            [StringLength(200, MinimumLength = 5)]
            public string EventName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Địa điểm là bắt buộc")]
            public string Location { get; set; } = string.Empty;

            [Required(ErrorMessage = "Ngày diễn ra là bắt buộc")]
            public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);

            public string? Genre { get; set; }
            public int? MaxAttendees { get; set; }
            public string? Description { get; set; }
            public bool IsPublished { get; set; } = false;
        }

        // [MỚI] DTO cho PricingTier
        public class PricingTierInput
        {
            [Required]
            public string TierName { get; set; } = string.Empty;
            [Required]
            [Range(0, double.MaxValue)]
            public decimal Price { get; set; }
            public int? AvailableTickets { get; set; }
            public string? Description { get; set; }
        }

        // [MỚI] DTO cho RequiredField
        public class RequiredFieldInput
        {
            [Required]
            public string FieldName { get; set; } = string.Empty;
            [Required]
            public string FieldType { get; set; } = "Text"; // "Text", "Email", "Checkbox"
            public bool IsRequired { get; set; } = true;
            public int DisplayOrder { get; set; }
        }

        public class AiRequest
        {
            public string? Name { get; set; }
            public string? Keywords { get; set; }
        }
        #endregion

        public void OnGet()
        {
            // [MỚI] Thêm 1-2 trường/vé mẫu để Admin dễ hình dung
            if (!PricingTiers.Any())
            {
                PricingTiers.Add(new PricingTierInput { TierName = "Vé Tiêu Chuẩn", Price = 100000, AvailableTickets = 100 });
            }

            if (!RequiredFields.Any())
            {
                RequiredFields.Add(new RequiredFieldInput { FieldName = "Họ và Tên", FieldType = "Text", IsRequired = true, DisplayOrder = 1 });
                RequiredFields.Add(new RequiredFieldInput { FieldName = "Email", FieldType = "Email", IsRequired = true, DisplayOrder = 2 });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var musicEvent = new MusicEvent
            {
                EventName = Input.EventName,
                Location = Input.Location,
                EventDate = Input.EventDate,
                Genre = Input.Genre,
                MaxAttendees = Input.MaxAttendees,
                Description = Input.Description,
                IsPublished = Input.IsPublished,
                CreatedByUserID = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            // [MỚI] Map các DTOs từ form sang Model
            musicEvent.PricingTiers = PricingTiers.Select(p => new PricingTier
            {
                TierName = p.TierName,
                Price = p.Price,
                AvailableTickets = p.AvailableTickets,
                Description = p.Description
            }).ToList();

            musicEvent.RequiredFields = RequiredFields.Select(f => new RequiredField
            {
                FieldName = f.FieldName,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                DisplayOrder = f.DisplayOrder
            }).ToList();

            try
            {
                var eventId = await _repository.CreateEventAsync(musicEvent);
                _logger.LogInformation("Sự kiện {EventId} đã được tạo bởi {UserName}", eventId, user.UserName);

                TempData["SuccessMessage"] = "Tạo sự kiện thành công!"; // Thêm thông báo
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sự kiện");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo sự kiện.");
                return Page();
            }
        }

        public async Task<JsonResult> OnPostGenerateDescriptionAsync([FromBody] AiRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Keywords))
            {
                return new JsonResult(new { success = false, error = "Tên sự kiện và từ khóa là bắt buộc." });
            }

            try
            {
                var description = await _aiService.GenerateEventDescriptionAsync(request.Name, request.Keywords);
                return new JsonResult(new { success = true, description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mô tả AI");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}