using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MusicEventManagementSystem.Models;
using MusicEventManagementSystem.Services.Interfaces;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // [SỬA LỖI] Thêm using cho List
using Microsoft.Extensions.Logging; // [SỬA LỖI] Thêm using cho ILogger
using System.Linq; // [SỬA LỖI] Thêm using cho .Select, .OrderBy
using System; // [SỬA LỖI] Thêm using cho DateTime, Exception

namespace MusicEventManagementSystem.Pages.Admin.Events
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        // [SỬA LỖI 1] Phục hồi các thuộc tính (fields) đã bị mất
        private readonly IMusicEventRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IMusicEventRepository repository,
            UserManager<ApplicationUser> userManager,
            ILogger<EditModel> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        // [SỬA LỖI 2] Phục hồi các thuộc tính BindProperty cho 2 danh sách
        [BindProperty]
        public List<PricingTierInput> PricingTiers { get; set; } = new List<PricingTierInput>();

        [BindProperty]
        public List<RequiredFieldInput> RequiredFields { get; set; } = new List<RequiredFieldInput>();

        #region Input DTOs
        public class InputModel
        {
            [Required]
            public int EventID { get; set; }

            [Required(ErrorMessage = "Event name is required")]
            [StringLength(200, MinimumLength = 5)]
            public string EventName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Location is required")]
            public string Location { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event date is required")]
            public DateTime EventDate { get; set; }

            public string? Genre { get; set; }
            public int? MaxAttendees { get; set; }
            public string? Description { get; set; }
            public bool IsPublished { get; set; }

            // [THÊM TỪ LẦN TRƯỚC] Thuộc tính ImageUrl đã thêm
            public string? ImageUrl { get; set; }

            public string CreatedByUserID { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        // [SỬA LỖI 3] Phục hồi class PricingTierInput
        public class PricingTierInput
        {
            public int PricingTierID { get; set; }

            [Required]
            public string TierName { get; set; } = string.Empty;
            [Required]
            [Range(0, double.MaxValue)]
            public decimal Price { get; set; }
            public int? AvailableTickets { get; set; }
            public string? Description { get; set; }
        }

        // [SỬA LỖI 4] Phục hồi class RequiredFieldInput
        public class RequiredFieldInput
        {
            public int FieldID { get; set; }

            [Required]
            public string FieldName { get; set; } = string.Empty;
            [Required]
            public string FieldType { get; set; } = "Text";
            public bool IsRequired { get; set; } = true;
            public int DisplayOrder { get; set; }
        }
        #endregion

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicEvent = await _repository.GetEventWithDetailsAsync(id.Value);

            if (musicEvent == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                EventID = musicEvent.EventID,
                EventName = musicEvent.EventName,
                Location = musicEvent.Location,
                EventDate = musicEvent.EventDate,
                Genre = musicEvent.Genre,
                MaxAttendees = musicEvent.MaxAttendees,
                Description = musicEvent.Description,
                IsPublished = musicEvent.IsPublished,
                CreatedByUserID = musicEvent.CreatedByUserID,
                CreatedDate = musicEvent.CreatedDate,

                // [THÊM TỪ LẦN TRƯỚC] Tải ImageUrl từ database
                ImageUrl = musicEvent.ImageUrl
            };

            // [SỬA LỖI 5] Phục hồi logic tải PricingTiers
            PricingTiers = musicEvent.PricingTiers.Select(p => new PricingTierInput
            {
                PricingTierID = p.PricingTierID,
                TierName = p.TierName,
                Price = p.Price,
                AvailableTickets = p.AvailableTickets,
                Description = p.Description
            }).ToList();

            // [SỬA LỖI 6] Phục hồi logic tải RequiredFields
            RequiredFields = musicEvent.RequiredFields.Select(f => new RequiredFieldInput
            {
                FieldID = f.FieldID,
                FieldName = f.FieldName,
                FieldType = f.FieldType,
                IsRequired = f.IsRequired,
                DisplayOrder = f.DisplayOrder
            }).OrderBy(f => f.DisplayOrder).ToList();

            return Page();
        }

        // [FIX HERE]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var eventToUpdate = new MusicEvent
            {
                EventID = Input.EventID,
                EventName = Input.EventName,
                Location = Input.Location,
                EventDate = Input.EventDate,
                Genre = Input.Genre,
                MaxAttendees = Input.MaxAttendees,
                Description = Input.Description,
                IsPublished = Input.IsPublished,

                // [THÊM TỪ LẦN TRƯỚC] Lưu ImageUrl khi post
                ImageUrl = Input.ImageUrl,

                CreatedByUserID = Input.CreatedByUserID, // Keep the old value
                CreatedDate = Input.CreatedDate,         // Keep the old value
                LastModifiedDate = DateTime.UtcNow,      // Update date

                // [SỬA LỖI 7] Phục hồi logic map PricingTiers
                PricingTiers = PricingTiers.Select(p => new PricingTier
                {
                    PricingTierID = p.PricingTierID,
                    TierName = p.TierName,
                    Price = p.Price,
                    AvailableTickets = p.AvailableTickets,
                    Description = p.Description,
                    EventID = Input.EventID // <-- [FIX HERE]
                }).ToList(),

                // [SỬA LỖI 8] Phục hồi logic map RequiredFields
                RequiredFields = RequiredFields.Select(f => new RequiredField
                {
                    FieldID = f.FieldID,
                    FieldName = f.FieldName,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    DisplayOrder = f.DisplayOrder,
                    EventID = Input.EventID // <-- [AND FIX HERE]
                }).ToList()
            };

            try
            {
                await _repository.UpdateEventAsync(eventToUpdate);
            }
            // [SỬA LỖI 9] Phục hồi các khối 'catch' đã bị mất
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error while updating event {EventId}", Input.EventID);
                ModelState.AddModelError(string.Empty, "The data was modified by another user. Please reload the page.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", Input.EventID);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the event.");
                return Page();
            }

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}