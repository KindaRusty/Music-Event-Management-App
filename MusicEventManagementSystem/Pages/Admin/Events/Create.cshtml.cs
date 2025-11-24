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
        [BindProperty]
        public List<PricingTierInput> PricingTiers { get; set; } = new List<PricingTierInput>();
        [BindProperty]
        public List<RequiredFieldInput> RequiredFields { get; set; } = new List<RequiredFieldInput>();
        #region Input DTOs
        public class InputModel
        {
            [Required(ErrorMessage = "Event name is required")]
            [StringLength(200, MinimumLength = 5)]
            public string EventName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Location is required")]
            public string Location { get; set; } = string.Empty;

            [Required(ErrorMessage = "Event date is required")]
            public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);

            public string? Genre { get; set; }
            public int? MaxAttendees { get; set; }
            public string? Description { get; set; }
            public bool IsPublished { get; set; } = false;
        }

        // DTO for PricingTier
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

        // DTO for RequiredField
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
            //Add 1-2 sample fields/tiers for Admin visualization
            if (!PricingTiers.Any())
            {
                PricingTiers.Add(new PricingTierInput { TierName = "Standard Ticket", Price = 100000, AvailableTickets = 100 });
            }

            if (!RequiredFields.Any())
            {
                RequiredFields.Add(new RequiredFieldInput { FieldName = "Full Name", FieldType = "Text", IsRequired = true, DisplayOrder = 1 });
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

            //Map DTOs from form to Model
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
                _logger.LogInformation("Event {EventId} was created by {UserName}", eventId, user.UserName);

                TempData["SuccessMessage"] = "Event created successfully!"; // Add message
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the event.");
                return Page();
            }
        }

        public async Task<JsonResult> OnPostGenerateDescriptionAsync([FromBody] AiRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Keywords))
            {
                return new JsonResult(new { success = false, error = "Event name and keywords are required." });
            }

            try
            {
                var description = await _aiService.GenerateEventDescriptionAsync(request.Name, request.Keywords);
                return new JsonResult(new { success = true, description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI description");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}